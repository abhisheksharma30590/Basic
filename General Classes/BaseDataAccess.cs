using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPMFeedBackPortalDataAccess
{
    public class BaseDataAccess
    {
        /// <summary>
        /// The connection string
        /// </summary>
        protected static string connectionString = ConfigurationManager.ConnectionStrings["IPMFeedBaack"].ConnectionString;

        /// <summary>
        /// Gets or sets the return_ value.
        /// </summary>
        /// <value>
        /// The return_ value.
        /// </value>
        public int Return_Value { get; set; }

        #region Execute Inline SQL Query
        /// <summary>
        /// Creates the SQL query.
        /// </summary>
        /// <param name="sqlQuery">The SQL query.</param>
        /// <returns></returns>
        protected bool CreateSqlQuery(string sqlQuery)
        {
            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                SqlCommand com = new SqlCommand(sqlQuery, con);
                con.Open();
                com.ExecuteNonQuery();
                con.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Selects the SQL query.
        /// </summary>
        /// <param name="sqlQuery">The SQL query.</param>
        /// <returns></returns>
        protected static DataTable SelectSqlQuery(string sqlQuery)
        {
            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                SqlCommand com = new SqlCommand(sqlQuery, con);
                SqlDataAdapter da = new SqlDataAdapter(com);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
        }
        #endregion

        #region Execute Non Query
        /// <summary>
        /// Executes the stored procedure.
        /// </summary>
        /// <param name="spName">Name of the sp.</param>
        /// <returns></returns>
        protected bool ExecuteStoredProcedure(string spName)
        {
            try
            {
                return ExecuteStoredProcedure(spName, (Hashtable)null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Executes the stored procedure.
        /// </summary>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="paramTable">The parameter table.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">
        /// ErrorDuplicateName
        /// or
        /// </exception>
        protected bool ExecuteStoredProcedure(string spName, Hashtable paramTable)
        {
            // Get the command.
            SqlCommand command = GetSqlCommand(spName, paramTable);
            try
            {
                command.Connection.Open();
                command.ExecuteNonQuery();
                if (command.Parameters.Contains("@Return_Value"))
                {
                    this.Return_Value = Convert.ToInt32(command.Parameters["@Return_Value"].Value);
                }
                else
                {
                    this.Return_Value = 0;
                }

                if (command != null && command.Connection != null)
                {
                    command.Connection.Close();
                }
                command.Dispose();
                return this.Return_Value == -1 ? false : true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (command != null && command.Connection != null)
                {
                    command.Connection.Close();
                }
            }
        }

        /// <summary>
        /// Executes the stored procedure.
        /// </summary>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="paramTable">The parameter table.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="tran">The tran.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">
        /// ErrorDuplicateName
        /// or
        /// ConfiguationExist
        /// </exception>
        protected bool ExecuteStoredProcedure(string spName, Hashtable paramTable, SqlConnection connection, SqlTransaction tran)
        {
            // Get the command.
            SqlCommand command = GetSqlCommand(connection, tran, spName, paramTable);
            try
            {
                command.ExecuteNonQuery();
                if (command.Parameters.Contains("@Return_Value"))
                {
                    this.Return_Value = Convert.ToInt32(command.Parameters["@Return_Value"].Value);
                }
                else
                {
                    this.Return_Value = 0;
                }
                command.Dispose();
                return this.Return_Value == -1 ? false : true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region GetDataSet - 3 methods.
        /// <summary>
        /// Gets the data set.
        /// </summary>
        /// <param name="spName">Name of the sp.</param>
        /// <returns></returns>
        protected DataSet GetDataSet(string spName)
        {
            try
            {
                return GetDataSet(spName, (Hashtable)null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the data set.
        /// </summary>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="paramTable">The parameter table.</param>
        /// <returns></returns>
        protected DataSet GetDataSet(string spName, Hashtable paramTable)
        {
            try
            {
                DataSet ds = GetDataSet(spName, paramTable, spName);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the data set.
        /// </summary>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="paramTable">The parameter table.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Error getting data row:  + spName</exception>
        protected DataSet GetDataSet(string spName, Hashtable paramTable, string tableName)
        {
            DataSet data = new DataSet();
            using (SqlCommand command = GetSqlCommand(spName, paramTable))
            {
                try
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        command.Connection.Open();
                        adapter.Fill(data, tableName);
                        if (command.Parameters.Contains("@Return_Value"))
                        {
                            this.Return_Value = Convert.ToInt32(command.Parameters["@Return_Value"].Value);
                        }
                        else
                        {
                            this.Return_Value = 0;
                        }
                        adapter.Dispose();
                    }
                    command.Dispose();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (command != null && command.Connection != null)
                    {
                        command.Connection.Close();
                    }
                }
            }
            return data;
            //SqlCommand command = GetSqlCommand(spName, paramTable);

        }
        #endregion

        #region GetDataRow
        /// <summary>
        /// Gets the data row.
        /// </summary>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="paramTable">The parameter table.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Error getting data row:  + spName</exception>
        protected DataRow GetDataRow(string spName, Hashtable paramTable)
        {
            DataRow row = null;
            try
            {
                DataSet data = GetDataSet(spName, paramTable);
                if (data != null && data.Tables[spName] != null && data.Tables[spName].Rows.Count == 1)
                {
                    row = data.Tables[spName].Rows[0];
                }

                return row;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region GetDataView - 2 methods.
        /// <summary>
        /// Gets the data view.
        /// </summary>
        /// <param name="spName">Name of the sp.</param>
        /// <returns></returns>
        protected DataView GetDataView(string spName)
        {
            try
            {
                return GetDataView(spName, (Hashtable)null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the data view.
        /// </summary>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="paramTable">The parameter table.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Error getting data view:  + spName</exception>
        protected DataView GetDataView(string spName, Hashtable paramTable)
        {
            // Get the new records from database.
            DataView view = null;
            try
            {
                DataSet data = GetDataSet(spName, paramTable);
                if (data != null)
                {
                    if (data.Tables.Count > 0)
                    {
                        if (data.Tables[spName] != null)
                        {
                            view = data.Tables[spName].DefaultView;
                        }
                    }
                }

                return view;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Private Method GetSQLCommand
        /// <summary>
        /// Gets the SQL command.
        /// </summary>
        /// <param name="spName">Name of the sp.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Error getting Sql command:  + spName</exception>
        private SqlCommand GetSqlCommand(string spName)
        {
            SqlCommand command = null;
            try
            {
                using (command = new SqlCommand())
                {
                    // Create the SQL Command object.
                    command.Connection = new SqlConnection(connectionString);
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = spName;

                }
                return command;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the SQL command.
        /// </summary>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="paramTable">The parameter table.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Error getting Sql command:  + spName</exception>
        private SqlCommand GetSqlCommand(string spName, Hashtable paramTable)
        {
            try
            {
                SqlCommand command = null;

                using (command = GetSqlCommand(spName))
                {
                    if (paramTable != null)
                    {
                        IDictionaryEnumerator en = paramTable.GetEnumerator();
                        while (en.MoveNext())
                        {
                            if (en.Value == null)
                            {
                                command.Parameters.AddWithValue("@" + Convert.ToString(en.Key).Trim(), en.Value);
                            }
                            else if (en.Value.GetType() == typeof(byte[]))
                            {
                                command.Parameters.AddWithValue("@" + Convert.ToString(en.Key).Trim(), en.Value);
                                command.Parameters[command.Parameters.Count - 1].DbType = DbType.Binary;
                            }
                            else
                            {
                                command.Parameters.AddWithValue("@" + Convert.ToString(en.Key).Trim(), Convert.ToString(en.Value).Trim());
                            }
                        }

                        command.Parameters.AddWithValue("@Return_Value", 0);
                        command.Parameters[command.Parameters.Count - 1].Direction = ParameterDirection.ReturnValue;
                    }
                }
                return command;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the SQL command.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="paramTable">The parameter table.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Error getting Sql command:  + spName</exception>
        protected SqlCommand GetSqlCommand(SqlConnection connection, SqlTransaction transaction, string spName, Hashtable paramTable)
        {
            SqlCommand command = null;
            try
            {
                command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = spName;
                if (paramTable != null)
                {
                    IDictionaryEnumerator en = paramTable.GetEnumerator();
                    while (en.MoveNext())
                    {
                        command.Parameters.AddWithValue("@" + en.Key.ToString().Trim(), en.Value.ToString().Trim());
                    }
                }

                command.Parameters.AddWithValue("@Return_Value", 0);
                command.Parameters[command.Parameters.Count - 1].Direction = ParameterDirection.ReturnValue;
                return command;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        # region SqlBulk Copy

        /// <summary>
        /// Performs the bulk copy.
        /// </summary>
        /// <param name="dtTable">The dt table.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        protected bool PerformBulkCopy(DataTable dtTable, string tableName)
        {
            try
            {
                using (SqlConnection destinationConnection = new SqlConnection(connectionString))
                {
                    //// open the connection
                    destinationConnection.Open();
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(destinationConnection))
                    {
                        ////bulkCopy.BatchSize = 500;
                        ////bulkCopy.NotifyAfter = 1000;
                        ////bulkCopy.SqlRowsCopied += new SqlRowsCopiedEventHandler(bulkCopy_SqlRowsCopied);
                        bulkCopy.DestinationTableName = tableName;
                        bulkCopy.WriteToServer(dtTable);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Updates the bulk copy.
        /// </summary>
        /// <param name="dtTable">The dt table.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="updateById">The update by identifier.</param>
        /// <param name="updatebyColumn">The updateby column.</param>
        /// <returns></returns>
        protected bool UpdateBulkCopy(DataTable dtTable, string tableName, int updateById = 0, string updatebyColumn = "")
        {
            try
            {
                string query = "UPDATE " + tableName + " SET ";
                foreach (DataRow dr in dtTable.Rows)
                {
                    foreach (DataColumn dc in dtTable.Columns)
                    {
                        query += dc.ColumnName + "=" + dr[dc];
                    }

                    if (updateById > 0 && !string.IsNullOrEmpty(updatebyColumn))
                    {
                        query += " WHERE " + updatebyColumn + "=" + updateById;
                    }

                    this.CreateSqlQuery(query);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Handles the SqlRowsCopied event of the bulkCopy control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SqlRowsCopiedEventArgs"/> instance containing the event data.</param>
        private static void BulkCopy_SqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            if (RowCopied != null)
            {
                RowCopied(sender, e);
            }
        }

        /// <summary>
        /// Occurs when [row copied].
        /// </summary>
        public static event RowCopiedEventHandler RowCopied;

        /// <summary>
        /// RowCopiedEventHandler
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SqlRowsCopiedEventArgs"/> instance containing the event data.</param>
        public delegate void RowCopiedEventHandler(object sender, SqlRowsCopiedEventArgs e);

        #endregion

        /// <summary>
        /// Executes the data table stored procedure.
        /// </summary>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="paramTable">The parameter table.</param>
        /// <param name="dataTable">The data table.</param>
        /// <param name="dataTableParamName">Name of the data table parameter.</param>
        /// <returns></returns>
        public bool ExecuteDataTableStoredProcedure(string spName, Hashtable paramTable, DataTable dataTable, string dataTableParamName)
        {
            SqlCommand command = null;
            try
            {
                command = GetSqlCommand(spName);
                if (paramTable != null)
                {
                    IDictionaryEnumerator en = paramTable.GetEnumerator();
                    while (en.MoveNext())
                    {
                        if (en.Value == null)
                        {
                            command.Parameters.AddWithValue("@" + Convert.ToString(en.Key).Trim(), en.Value);
                        }
                        else if (en.Value.GetType() == typeof(byte[]))
                        {
                            command.Parameters.AddWithValue("@" + Convert.ToString(en.Key).Trim(), en.Value);
                            command.Parameters[command.Parameters.Count - 1].DbType = DbType.Binary;
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@" + Convert.ToString(en.Key).Trim(), Convert.ToString(en.Value).Trim());
                        }
                    }

                    command.Parameters.AddWithValue("@Return_Value", 0);
                    command.Parameters[command.Parameters.Count - 1].Direction = ParameterDirection.ReturnValue;
                }
                command.Parameters.AddWithValue("@" + dataTableParamName, dataTable);
                command.Connection.Open();
                command.ExecuteNonQuery();
                if (command.Parameters.Contains("@Return_Value"))
                {
                    this.Return_Value = Convert.ToInt32(command.Parameters["@Return_Value"].Value);
                }
                else
                {
                    this.Return_Value = 0;
                }
                if (command != null && command.Connection != null)
                {
                    command.Connection.Close();
                }
                command.Dispose();
                return this.Return_Value == -1 ? false : true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (command != null && command.Connection != null)
                {
                    command.Connection.Close();
                }
            }
        }
    }
}
