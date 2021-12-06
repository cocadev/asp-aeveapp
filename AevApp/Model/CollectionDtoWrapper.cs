using System.Collections.Generic;

namespace AevApp.Model
{
    public class CollectionDtoWrapper<T>
    {
        public long Total { get; set; }

        public IEnumerable<T> Items { get; set; }
    }
}
