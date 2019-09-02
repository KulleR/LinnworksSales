﻿using LinnworksSales.Data.Data.Repository.Interfaces;
using LinnworksSales.Data.Enums;
using LinnworksSales.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LinnworksSales.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        /// <summary>
        /// Provide access to instructors in database
        /// </summary>
        public IRegionRepository RegionRepository { get; set; }

        public ICountryRepository CountryRepository { get; set; }

        public IItemTypeRepository ItemTypeRepository { get; set; }

        public ISaleRepository SaleRepository { get; set; }
        /// <summary>
        /// Provide access to object mapper
        /// </summary>
        public ICommonMapper Mapper { get; }

        public SalesController(ISaleRepository orderRepository, IRegionRepository regionRepository,
            ICountryRepository countryRepository, IItemTypeRepository itemTypeRepository, ICommonMapper mapper)
        {
            SaleRepository = orderRepository;
            RegionRepository = regionRepository;
            CountryRepository = countryRepository;
            ItemTypeRepository = itemTypeRepository;
            Mapper = mapper;
        }

        /// <summary>
        /// Action that only support the HTTP GET method wich return all instructors. 
        /// </summary>
        /// <returns>Return JSON array with StatucCode 200</returns>
        [HttpGet]
        public IActionResult Get(int? page, int count, string sortColumn, string direction = "asc", string country = "")
        {
            PageEntitiesContainer<Sale> pageEntities = new PageEntitiesContainer<Sale>(
                SaleRepository.GetAll().Include(s => s.Country).Include(s => s.ItemType), SaleRepository);

            pageEntities.RegisterFilter(new Filter(typeof(Country), country));

            switch (sortColumn)
            {
                case "orderDate":
                    pageEntities.OrderBy(s => s.OrderDate, direction);
                    break;
            }

            pageEntities.SetPage(page, count > 0 ? count : 50);

            return Ok(Mapper.Map<PageEntitiesContainerDto<SaleDto>>(pageEntities));
        }

        /// <summary>
        /// Action that only support the HTTP GET method which return instructor by id. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>400 Bad Request if instructor by id doesn`t exist or 201 Created if success</returns>
        [HttpGet("{id}", Name = "GetOrder")]
        public async Task<IActionResult> Get(int id)
        {
            Sale dbOrder = await SaleRepository.GetAsync(id);

            if (dbOrder == null)
            {
                return BadRequest();
            }

            return Ok(dbOrder);
        }

        /// <summary>
        /// Action that only support the HTTP POST method, which create a new instructor. 
        /// </summary>
        /// <param name="instructor">New instructor</param>
        /// <returns>400 Bad Request if argument is null or 201 Created if success</returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]SaleDto instructor)
        {
            if (instructor == null)
            {
                return BadRequest();
            }

            Sale orderToSave = Mapper.Map<Sale>(instructor);
            //if (!await SaleRepository.SaveAsync(orderToSave))
            //{
            //    throw new Exception("Creating a instructor failed on save.");
            //}

            SaleDto orderToReturn = Mapper.Map<SaleDto>(orderToSave);
            return CreatedAtRoute("GetOrder", new { id = orderToReturn.Id }, orderToReturn);
        }

        /// <summary>
        /// Action that only support the HTTP PUT method, which update instructor. 
        /// </summary>
        /// <param name="id">Instructor ID to be updated</param>
        /// <param name="sale">Instructor to update</param>
        /// <returns>400 Bad Request if instructor by id doesn`t exist or 204 No Content if success</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody]SalePutDto sale)
        {
            Sale dbSale = await SaleRepository.GetAsync(id);

            if (sale == null)
            {
                return BadRequest();
            }

            Mapper.Map(sale, dbSale);

            if (!SaleRepository.Update(dbSale))
            {
                throw new Exception($"Updating a instructor {id} failed on save.");
            }

            return NoContent();
        }

        /// <summary>
        /// Action that only support the HTTP PUT method, which update instructor. 
        /// </summary>
        /// <param name="id">Instructor ID to be deleted</param>
        /// <returns>400 Bad Request if instructor by id doesn`t exist or 204 No Content if success</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            Sale order = await SaleRepository.GetAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            if (!SaleRepository.Delete(order))
            {
                throw new Exception($"Deleting a instructor {id} failed on save.");
            }
            return NoContent();
        }

        [HttpPost("Import")]
        public async Task<IActionResult> Import(List<IFormFile> files)
        {
            string[] read;
            char[] seperators = { ',' };
            long size = files.Sum(f => f.Length);
            List<SaleDenormalizedModel> salesDenormal = new List<SaleDenormalizedModel>();

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    using (var stream = formFile.OpenReadStream())
                    using (var streamReader = new StreamReader(stream))
                    {
                        string data = streamReader.ReadLine();

                        while (!streamReader.EndOfStream)
                        {
                            data = streamReader.ReadLine();
                            read = data.Split(seperators, StringSplitOptions.RemoveEmptyEntries);
                            try
                            {
                                salesDenormal.Add(new SaleDenormalizedModel
                                {
                                    Region = read[0],
                                    Country = read[1],
                                    ItemType = read[2],
                                    SalesChanel = read[3],
                                    OrderPriority = read[4],
                                    OrderDate = read[5],
                                    OrderId = read[6],
                                    ShipDate = read[7],
                                    UnitsSold = read[8],
                                    UnitPrice = read[9],
                                    UnitCost = read[10]
                                });
                            }
                            catch (IndexOutOfRangeException)
                            {
                                continue;
                            }
                        }

                        List<Region> regions = salesDenormal.Select(s => s.Region).Distinct().Select(s => new Region() { Name = s }).ToList();
                        await RegionRepository.BulkMergeAsync(regions);

                        List<Country> countries = salesDenormal.GroupBy(s => s.Country).
                            Select(s => new Country()
                            {
                                Name = s.Key,
                                Region = regions.FirstOrDefault(r => r.Name == s.FirstOrDefault()?.Region)
                            }).ToList();
                        await CountryRepository.BulkMergeAsync(countries);

                        List<ItemType> itemTypes = salesDenormal.Select(s => s.ItemType).Distinct().Select(s => new ItemType() { Name = s }).ToList();
                        await ItemTypeRepository.BulkMergeAsync(itemTypes);

                        List<Sale> sales = salesDenormal.Select(s => new Sale()
                        {
                            Country = countries.FirstOrDefault(r => r.Name == s.Country),
                            ItemType = itemTypes.FirstOrDefault(t => t.Name == s.ItemType),
                            SalesChanel = Enum.Parse<SalesChanel>(s.SalesChanel),
                            OrderDate = DateTime.Parse(s.OrderDate),
                            OrderPriority = Enum.Parse<OrderPriority>(s.OrderPriority),
                            ShipDate = DateTime.Parse(s.ShipDate),
                            UnitCost = decimal.Parse(s.UnitCost),
                            UnitPrice = decimal.Parse(s.UnitPrice),
                            UnitsSold = int.Parse(s.UnitsSold),
                            OrderId = int.Parse(s.OrderId)
                        }).ToList();
                        await SaleRepository.BulkMergeAsync(sales, options => options.ColumnPrimaryKeyExpression = c => c.OrderId);
                    }
                }
            }

            // process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(new { count = files.Count, size });
        }
    }
}
