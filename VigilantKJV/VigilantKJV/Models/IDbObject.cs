using System;
using System.Collections.Generic;
using System.Text;

namespace VigilantKJV.Models
{
   public interface IDbObject
    {

       string FriendlyLabel { get; }
        int Id { get;  set;}
    }
}
