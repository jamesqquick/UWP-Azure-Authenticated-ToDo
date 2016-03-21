using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;
using Windows.Security.Credentials;
using System.Collections.Generic;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace AuthenticatedTodoItem
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class HomePage : Page
	{

		List<ToDoItem> items = new List<ToDoItem>();

		public HomePage()
		{
			this.InitializeComponent();
			this.Loaded += HomePage_Loaded;
			
		}

		private void HomePage_Loaded(object sender, RoutedEventArgs e)
		{
			TitleText.Text = App.mobileService.CurrentUser.UserId;
			LoadToDoItems();
		}

		private async void LoadToDoItems()
		{
			items.Clear();
			try
			{
				items = await App.mobileService.GetTable<ToDoItem>()
					.Where(ToDoItem => ToDoItem.UserId == App.mobileService.CurrentUser.UserId)
					.ToListAsync();
				ItemsList.ItemsSource = items;
				//Update ListView to show no selected Item
				ItemsList.SelectedIndex = -1;
			}
			catch(Exception ex)
			{
				Debug.WriteLine("Table Query Exception: " + ex.Message);
				new Windows.UI.Popups.MessageDialog("Trouble Retrieving ToDo Itens");
			}
		}

		private async void LogoutButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				//Removed stored credentials for user
				PasswordVault vault = new PasswordVault();
				vault.Remove(vault.Retrieve("Custom", App.mobileService.CurrentUser.UserId));
				//Log out of MobileServiceClient
				await App.mobileService.LogoutAsync();
				Debug.WriteLine("Logout Successful");
				//Navigate to Login Page
				this.Frame.Navigate(typeof(LoginPage));
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Logout Exception: " + ex.Message);

			}
		}

		private async void AddButton_Click(object sender, RoutedEventArgs e)
		{
			//Create ContentDialog and Add title, ok button, and cancel button
			var dialog = new ContentDialog();
			dialog.Title = "Add New Item";
			dialog.PrimaryButtonText = "OK";
			dialog.SecondaryButtonText = "Cancel";

			//Create a textbox to be used as Content for dialog and take user input
			TextBox entryText = new TextBox();
			entryText.Margin = new Thickness(20);
			dialog.Content = entryText;

			//Show the Dialog and retrieve the result
			var result = await dialog.ShowAsync();
			if (result == ContentDialogResult.Primary)
			{
				//Check to see that the user has input something
				if (entryText.Text.Equals(""))
				{
					Debug.WriteLine("User did not add input");
					DisplayUserMessage("ToDoItem text cannot be empty");
				}
				//If the user did input something, then add the item to our table, and display Success dialog
				else
				{ 
					ToDoItem newItem = new ToDoItem { UserId = App.mobileService.CurrentUser.UserId, Text = entryText.Text };
					InsertItem(newItem);
					//Update the UI
					LoadToDoItems();
				}	
			}
		}

		private async void DeleteButton_Click(object sender, RoutedEventArgs e)
		{
			//Create ContentDialog and Add title, ok button, and cancel button
			var dialog = new ContentDialog();
			dialog.Title = "Add you sure you want to delete this item?";
			dialog.PrimaryButtonText = "Yes";
			dialog.SecondaryButtonText = "Cancel";

			//Show the Dialog and retrieve the result
			var result = await dialog.ShowAsync();
			
			if(result == ContentDialogResult.Primary)
			{
				DeleteItem((ToDoItem)ItemsList.SelectedItem);
			}
			
		}

		private async void EditButton_Click(object sender, RoutedEventArgs e)
		{
			ToDoItem temp = (ToDoItem)ItemsList.SelectedItem;

			//Create ContentDialog and Add title, ok button, and cancel button
			var dialog = new ContentDialog();
			dialog.Title = "Edit this item";
			dialog.PrimaryButtonText = "Save";
			dialog.SecondaryButtonText = "Cancel";

			//TextBox to edit the Text of the ToDoItem
			//Create a textbox to be used as Content for dialog and take user input
			TextBox entryText = new TextBox();
			entryText.Margin = new Thickness(20);
			entryText.Text = ((ToDoItem)ItemsList.SelectedItem).Text;
			dialog.Content = entryText;

			//Show the Dialog and retrieve the result
			var result = await dialog.ShowAsync();

			if (result == ContentDialogResult.Primary)
			{
				if (!entryText.Text.Equals(""))
				{
					temp.Text = entryText.Text;
					EditItem(temp);
				}
				else
				{
					Debug.WriteLine("User did not add input");
					DisplayUserMessage("ToDoItem text cannot be empty");
				}
				
			}
		}

		public async void EditItem(ToDoItem temp)
		{
			try
			{
				await App.mobileService.GetTable<ToDoItem>().UpdateAsync(temp);
				Debug.WriteLine("Item Update Success");
				DisplayUserMessage("Item successfully updated");
				//Update UI
				LoadToDoItems();
			}
			catch(Exception ex)
			{
				Debug.WriteLine("Item Update Error: " + ex.Message);
			}
        }

		public async void InsertItem(ToDoItem temp)
		{
			try
			{
				await App.mobileService.GetTable<ToDoItem>().InsertAsync(temp);
				Debug.WriteLine("Table Insert Success");
				DisplayUserMessage("Item Successfully added");
				//Update the UI
				LoadToDoItems();
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Table Insert Error Occurred: " + ex.Message);
			}
		}

		public async void DeleteItem(ToDoItem temp)
		{
			try
			{
				await App.mobileService.GetTable<ToDoItem>().DeleteAsync(temp);
				Debug.WriteLine("Item Delete Success");
				DisplayUserMessage("Successfully deleted item.");
				//Update UI
				LoadToDoItems();
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Error Deleting Item: " + ex.Message);
			}
		}

		private void ItemsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			//Toggle the Delete and Edit Button when an item is selected
			if (((ListView)sender).SelectedIndex == -1)
			{
				DeleteButton.IsEnabled = false;
				EditButton.IsEnabled = false;
			}
			else
			{
				DeleteButton.IsEnabled = true;
				EditButton.IsEnabled = true;
			}
		}

		private async void DisplayUserMessage(string message)
		{
			await new Windows.UI.Popups.MessageDialog(message).ShowAsync();
		}
	
	}
}
