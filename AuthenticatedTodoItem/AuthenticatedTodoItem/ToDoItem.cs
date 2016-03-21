using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticatedTodoItem
{
	public class ToDoItem
	{
		[JsonProperty (PropertyName = "id")]
		public string Id { get; set; }

		[JsonProperty (PropertyName = "text")]
		public string Text { get; set; }

		[JsonProperty(PropertyName = "complete")]
		public bool Complete { get; set; }

		[JsonProperty(PropertyName = "userId")]
		public string UserId { get; set; }
	}
}
