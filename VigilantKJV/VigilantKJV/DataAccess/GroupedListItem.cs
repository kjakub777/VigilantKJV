using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace VigilantKJV.DataAccess
{
    public class GroupedListItem<TKey,TIEnumerable,TSource  >   where TIEnumerable:IEnumerable<TSource>
    {
        public TKey GroupKey { get; set; }

        TIEnumerable collection;
        public TIEnumerable Collection
        {
            get
            {
                return this.collection;
            }
            set => this.collection = value;
        }
        public GroupedListItem()
        {
             
        }
        public void SetItems(TIEnumerable collection)
        {
            this.collection=collection;
        }

    }
}