using System;
 

namespace UvA.SPlusTools.Data.Entities
{
    [Serializable]
    public partial class Tag : SPlusObject
	{
		public Tag(College col) 
		{
			College = col;
			Object = col.Object.CreateTag();
			College.GetObject<Tag>(Object);
		}

		public Tag(College col, dynamic obj) 
		{
			College = col;
			Object = obj;
		}


	}
} 