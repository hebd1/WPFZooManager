using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace WPFZooManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection sqlConnection;

        public MainWindow()
        {
            InitializeComponent();

            // added configurationmanager from .. right click references then add using system.configuration
            // basic SQL query: select a.Location from Zoo a inner join ZooAnimal za on a.Id = za.ZooID where za.AnimalID = 1
            string connectionString = ConfigurationManager.ConnectionStrings["WPFZooManager.Properties.Settings.SQLDBConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(connectionString);
            ShowZoos();
            ShowAllAnimals();
        }

        /*
         * Populates the Zoo List Box using sqlServer
         */
        private void ShowZoos()
        {
            try
            {
                string query = "select * from Zoo";
                // sql data adapter is like an interface used to make tables usable by C# objects
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable zooTable = new DataTable();

                    sqlDataAdapter.Fill(zooTable);

                    ZooList.DisplayMemberPath = "Location"; // content of ListBox from DataTable   
                    ZooList.SelectedValuePath = "Id"; // value that should be delivered, when an item from ListBox is selected
                    ZooList.ItemsSource = zooTable.DefaultView; // reference to the data the listbox should populate/alter
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            
        }

        /**
         * Shows the animals associated with a selected view as recieved from the Zoo List box
         * 
         **/
        private void ShowAssociatedAnimals()
        {
            try
            {
                string query = " select a.Name from Animal a inner join ZooAnimal za on" +
                    " a.Id = za.AnimalID where za.ZooID = @ZooID";

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {

                    sqlCommand.Parameters.AddWithValue("@ZooID", ZooList.SelectedValue);

                    DataTable associatedAnimalTable = new DataTable();

                    sqlDataAdapter.Fill(associatedAnimalTable);

                    AnimalList.DisplayMemberPath = "Name";
                    AnimalList.SelectedValuePath = "Id";
                    AnimalList.ItemsSource = associatedAnimalTable.DefaultView;
                }

            } catch (Exception e)
            {
               // MessageBox.Show(e.ToString());
            }
        }

        // show all animals
        private void ShowAllAnimals()
        {
            try
            {
                string query = "select * from Animal";

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {
                    DataTable animalTable = new DataTable();
                    sqlDataAdapter.Fill(animalTable);

                    AllAnimalList.DisplayMemberPath = "Name";
                    AllAnimalList.SelectedValuePath = "Id";
                    AllAnimalList.ItemsSource = animalTable.DefaultView;

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

       
        }

        // zoo selector click
        private void ZooList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowAssociatedAnimals();
            ShowSelectedZooInTextBox();
        }

        // add new animal
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "insert into Animal values (@Name)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Name", TextBox.Text);
                sqlCommand.ExecuteScalar();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                // close connection
                sqlConnection.Close();
                ShowAllAnimals();
            }
        }

        // delete animal
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            try
            {
                // a different way to update the database
                string query = "delete from Animal where id = @Name";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                // open connection
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Name", AllAnimalList.SelectedValue);
                sqlCommand.ExecuteScalar();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                // close connection
                sqlConnection.Close();
                ShowAllAnimals();

            }
        }

        // add new zoo
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "insert into Zoo values (@Location)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Location", TextBox.Text);
                sqlCommand.ExecuteScalar();

            } catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                // close connection
                sqlConnection.Close();
                ShowZoos();
            }
        }

        // delete zoo
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                // a different way to update the database
                string query = "delete from Zoo where id = @ZooID";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                // open connection
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZooID", ZooList.SelectedValue);
                sqlCommand.ExecuteScalar();
              
            } catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                // close connection
                sqlConnection.Close();
                ShowZoos();

            }
        }

        // add animal to zoo
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "insert into ZooAnimal values (@ZooID, @AnimalID)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZooID", ZooList.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@AnimalID", AllAnimalList.SelectedValue);
                sqlCommand.ExecuteScalar();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                // close connection
                sqlConnection.Close();
                ShowAssociatedAnimals();
                ShowZoos();
            }
        }

        // remove animal from zoo
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete from ZooAnimal where AnimalID = @AnimalID and ZooID = @ZooID";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@AnimalID", AllAnimalList.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@ZooID", ZooList.SelectedValue);

                sqlCommand.ExecuteScalar();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                // close connection
                sqlConnection.Close();
                ShowAssociatedAnimals();
                ShowZoos();
            }
        }

        // update
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            //TODO: update database when element is updated in the text box
        }

        // show selected zoo in text box
        private void ShowSelectedZooInTextBox()
        {

            try
            {
                string query = "select location from Zoo where Id = @ZooID";

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {

                    sqlCommand.Parameters.AddWithValue("@ZooID", ZooList.SelectedValue);

                    DataTable zooDataTable = new DataTable();

                    sqlDataAdapter.Fill(zooDataTable);

                    //update textbox 
                    TextBox.Text = zooDataTable.Rows[0]["Location"].ToString();
                    
                }

            }
            catch (Exception ex)
            {
                // MessageBox.Show(e.ToString());
            }
        }

        // show selected animal in text box
        private void ShowSelectedAnimalInTextBox()
        {

            try
            {
                string query = "select Name from Animal where Id = @AnimalID";

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {

                    sqlCommand.Parameters.AddWithValue("@AnimalID", AllAnimalList.SelectedValue);

                    DataTable zooDataTable = new DataTable();

                    sqlDataAdapter.Fill(zooDataTable);

                    //update textbox 
                    TextBox.Text = zooDataTable.Rows[0]["Name"].ToString();

                }

            }
            catch (Exception ex)
            {
                // MessageBox.Show(e.ToString());
            }
        }

        // all animal list selection changed
        private void AllAnimalList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowSelectedAnimalInTextBox();
        }
    }
}
