﻿using System.Collections.Generic;

namespace LinnworksSales.Data.Models
{
    public class PageEntitiesContainerDto<TEntity>
    {
        public int TotalItemsCount { get; set; }
        public List<TEntity> OnePageItems { get; set; } = new List<TEntity>();
    }
}
