using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticatedTodoItem
{
	public class ToDoItemTableHelper
	{

		public static async Task<string> InsertItemAsync<T>(T temp)
		{
			string message = "";
			try
			{
				await App.mobileService.GetTable<T>().InsertAsync(temp);
				Debug.WriteLine("Table Insert Success");
				message = "Success";
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Table Insert Error Occurred: " + ex.Message);
				message = "Table Insert Error Occurred";
			}

			return message;
		}
		public static async Task<string> DeleteItemAsync<T>(T temp)
		{
			string message = "";
			try
			{
				await App.mobileService.GetTable<T>().DeleteAsync(temp);
				Debug.WriteLine("Item Delete Success");
				message = "Success";
			}
			catch (Exception ex)
			{
				message = "Error Deleted Item";
				Debug.WriteLine("Error Deleting Item: " + ex.Message);
			}
			return message;
		}

		public static async Task<string> EditItemAsync<T>(T temp)
		{
			string message = "";
			try
			{
				await App.mobileService.GetTable<T>().UpdateAsync(temp);
				Debug.WriteLine("Item Update Success");
				message = "Success";
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Item Update Error: " + ex.Message);
				message = "Item Update Error";
			}
			return message;
		}


	}
}
