using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Brighteye_Test
{
    public partial class MainWindow : Window
    {
        private MyDbContext? context;

        public MainWindow()
        {
            InitializeComponent();
            InitializeWindow();
        }

        private void InitializeWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            context = new MyDbContext();
        }

        private async void ClearAndFillTable1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ToggleUIElements(false);
                ToggleUIElementVisibility("dataGrid", false);

                await Task.Run(() =>
                {
                    var table1 = context?.Set<Table1>();
                    if (table1 != null)
                    {
                        ClearTable(table1);
                        FillTableWithRandomNumbers(table1);
                    }
                    else
                    {
                        MessageBox.Show("Table1 is null.");
                    }
                });

                MessageBox.Show("Table1 cleared and filled with random numbers.", "Info");
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException("filling Table1", ex);
            }
            finally
            {
                ToggleUIElements(true);
            }
        }

        private async void ClearSortAndFillTable2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ToggleUIElements(false);
                await Task.Run(() =>
                {
                    var table1 = context?.Set<Table1>();
                    var table2 = context?.Set<Table2>();

                    if (table1 != null && table1.Any())
                    {
                        if (table2 != null)
                        {
                            SortAndFillTable2(table2, table1, "Value");

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                dataGrid.ItemsSource = table2.ToList();
                            });

                            MessageBox.Show("Table2 cleared and filled with sorted numbers from Table1.","Info");
                        }
                        else
                        {
                            MessageBox.Show("Table2 has null value.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Source table (Table1) is empty or has null value. No data to sort and fill Table2.");
                    }
                });
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException("filling Table2", ex);
            }
            finally
            {
                ToggleUIElements(true);
                ToggleUIElementVisibility("dataGrid", true);
            }
        }

        private void ClearTable<TEntity>(DbSet<TEntity> table) where TEntity : class
        {
            table.RemoveRange(table);
            context?.SaveChanges();
        }

        private void FillTableWithRandomNumbers<TEntity>(DbSet<TEntity> table, string columnName = "Value") where TEntity : class, new()
        {
            var entityType = typeof(TEntity);
            var property = entityType.GetProperty(columnName);

            if (property == null)
            {
                throw new ArgumentException($"Column '{columnName}' does not exist in entity '{entityType.Name}'.");
            }

            for (int i = 1; i <= 10; i++)
            {
                var entity = new TEntity();

                if (property != null)
                {
                    property.SetValue(entity, HelperMethod.GenerateRandomValue());
                }

                table.Add(entity);
            }

            context?.SaveChanges();
        }

        private void SortAndFillTable2<TEntity, TSourceEntity>(DbSet<TEntity> destinationTable, DbSet<TSourceEntity> sourceTable, string columnName = "Value", bool descending = false)
            where TEntity : class, new()
            where TSourceEntity : class
        {
            var sortedEntities = sourceTable.SortTable(columnName, descending);
            ClearTable(destinationTable);
            context?.SaveChanges();

            foreach (var sortedEntity in sortedEntities)
            {
                var entity = new TEntity();
                var property = typeof(TEntity).GetProperty(columnName);
                var sourceHasProperty = HelperMethod.HasProperty(sortedEntity, columnName);

                if (property != null && sortedEntity != null && sourceHasProperty)
                {
                    var sourceValue = sortedEntity.GetType().GetProperty(columnName)?.GetValue(sortedEntity, null);

                    if (sourceValue != null && property.PropertyType == sourceValue.GetType())
                    {
                        property.SetValue(entity, sourceValue);
                        destinationTable.Add(entity);
                    }
                    else
                    {
                        try
                        {
                            var convertedValue = Convert.ChangeType(sourceValue, property.PropertyType);
                            property.SetValue(entity, convertedValue);
                            destinationTable.Add(entity);
                        }
                        catch (InvalidCastException ex)
                        {
                            ExceptionHandler.HandleException("filling Table2", ex);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("We have problems in columns in Table1 and Table2, please check tables.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
                }
            }

            context?.SaveChanges();
        }

        private void ToggleUIElements(bool isEnabled)
        {
            ToggleUIElementVisibility("myStackPanel", isEnabled);
            ToggleUIElementVisibility("spinner", !isEnabled);
        }

        private void ToggleUIElementVisibility(string elementName, bool isVisible)
        {
            var element = FindName(elementName) as UIElement;

            if (element != null)
            {
                element.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            context?.Dispose();
        }


    }
}
