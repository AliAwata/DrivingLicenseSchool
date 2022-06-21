using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Collections.Generic;

namespace DrivingSclApp.Models
{
    public partial class OraSync_TxConnectionProvider : IDisposable
    {
        protected bool _isDisposed;
        protected OracleConnection _txConnection;
        protected OracleTransaction _currTransaction;
        static string _connectionString;

        public OraSync_TxConnectionProvider()
        {
            Init();
        }

        private void Init()
        {
            _txConnection = new OracleConnection();
            _txConnection.ConnectionString = ConnectionString;
            _currTransaction = null;
            _isDisposed = false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!_isDisposed)
            {
                if (isDisposing)
                {
                    if (_currTransaction != null)
                    {
                        _currTransaction.Dispose();
                        _currTransaction = null;
                    }
                    if (_txConnection != null)
                    {
                        /*this will also rollback any pending transactions on this connection*/
                        _txConnection.Close();
                        _txConnection.Dispose();
                        _txConnection = null;
                    }
                }
            }
            _isDisposed = true;
        }

        public virtual void OpenConnection()
        {
            try
            {
                if (_txConnection.State == ConnectionState.Open)
                    throw new Exception("Connection is already open");

                _txConnection.Open();
                _currTransaction = null;
                _isDisposed = false;
            }
            catch
            {
                throw;
            }
        }

        public virtual void CloseConnection(bool commit)
        {
            if ((_txConnection == null) || (_txConnection.State != ConnectionState.Open))
                return;
            try
            {
                if ((_currTransaction != null) && commit)
                    _currTransaction.Commit();

                else if (_currTransaction != null)
                    _currTransaction.Rollback();

                if (_currTransaction != null)
                    _currTransaction.Dispose();

                _currTransaction = null;
                _txConnection.Close();
            }
            catch
            {
                throw;
            }
        }

        public virtual void BeginTransaction(string trans)
        {
            if (_currTransaction != null)
                throw new Exception("Transaction nesting not allowed");

            if ((_txConnection == null) || (_txConnection.State != ConnectionState.Open))
                throw new Exception("Connection not open");

            try
            {
                _currTransaction = _txConnection.BeginTransaction(IsolationLevel.ReadCommitted);
                _currTransaction.Save(trans);
            }
            catch
            {
                throw;
            }
        }

        public virtual void CommitTransaction()
        {
            if (_currTransaction == null)
                throw new Exception("No Transaction to commit");

            if ((_txConnection == null) || (_txConnection.State != ConnectionState.Open))
                throw new Exception("Connection not open");

            try
            {
                _currTransaction.Commit();
                _currTransaction.Dispose();
                _currTransaction = null;
            }
            catch
            {
                throw;
            }
        }

        public virtual void RollbackTransaction(string trans)
        {
            if (_currTransaction == null)
                throw new Exception("No Transaction to rollback");

            if ((_txConnection == null) || (_txConnection.State != ConnectionState.Open))
                throw new Exception("Connection not open");

            try
            {
                _currTransaction.Rollback(trans);
                _currTransaction.Dispose();
                _currTransaction = null;
            }
            catch
            {
                throw;
            }
        }

        public virtual OracleTransaction CurrentTransaction
        {
            get
            {
                return _currTransaction;
            }
        }

        public virtual OracleConnection Connection
        {
            get
            {
                return _txConnection;
            }
        }

        public static string ConnectionString
        {
            set { _connectionString = value; }
            get
            {
                if (!string.IsNullOrEmpty(_connectionString))
                    return _connectionString;
                return System.Configuration.ConfigurationManager.ConnectionStrings["DrivingSclEntity"].ConnectionString;
            }
        }
    }
    public partial class OraSync_BaseData : IDisposable
    {
        #region members
        protected OraSync_TxConnectionProvider _connectionProvider;
        static string _staticConnectionString;
        bool _isDisposed = false;
        #endregion

        #region initialisation
        public OraSync_BaseData()
        {
            Init();
        }

        private void Init()
        {
        }
        #endregion

        #region connection support
        public virtual OraSync_TxConnectionProvider ConnectionProvider
        {
            set
            {
                if (value == null)
                    throw new Exception("Connection provider cannot be null");

                _connectionProvider = value;
            }
        }

        public static OracleConnection StaticSqlConnection
        {
            get
            {
                OracleConnection staticConnection = new OracleConnection();
                staticConnection.ConnectionString = StaticConnectionString;
                return staticConnection;
            }
        }

        public static string StaticConnectionString
        {
            set { _staticConnectionString = value; }
            get
            {
                if (!string.IsNullOrEmpty(_staticConnectionString))
                    return _staticConnectionString;
                return System.Configuration.ConfigurationManager.ConnectionStrings["DrivingSclEntity"].ConnectionString;
            }
        }
        #endregion

        #region disposable interface support
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!_isDisposed)
            {
                if (isDisposing)
                {
                    if (_connectionProvider != null)
                    {
                        ((IDisposable)_connectionProvider).Dispose();
                        _connectionProvider = null;
                    }
                }
            }
            _isDisposed = true;
        }
        #endregion

        #region oracle parameter helpers
        protected virtual object UnboxOracleParam(object outParam)
        {
            Type t = outParam.GetType();
            if (t == typeof(OracleDecimal))
            {
                if (!((OracleDecimal)outParam).IsNull)
                    return (decimal)((OracleDecimal)outParam).Value;
                else
                    return null;
            }
            else if (t == typeof(OracleString))
            {
                if (!((OracleString)outParam).IsNull)
                    return (string)((OracleString)outParam).Value;
                else
                    return null;
            }
            else if (t == typeof(OracleIntervalYM))
            {
                if (!((OracleIntervalYM)outParam).IsNull)
                    return (Int64)((OracleIntervalYM)outParam).Value;
                else
                    return null;
            }
            else if (t == typeof(OracleIntervalDS))
            {
                if (!((OracleIntervalDS)outParam).IsNull)
                    return (TimeSpan)((OracleIntervalDS)outParam).Value;
                else
                    return null;
            }
            else if (t == typeof(OracleBinary))
            {
                if (!((OracleBinary)outParam).IsNull)
                    return (byte[])((OracleBinary)outParam).Value;
                else
                    return null;
            }
            else if (t == typeof(OracleDate))
            {
                if (!((OracleDate)outParam).IsNull)
                    return (DateTime)((OracleDate)outParam).Value;
                else
                    return null;
            }
            else if (t == typeof(OracleTimeStamp))
            {
                if (!((OracleTimeStamp)outParam).IsNull)
                    return (DateTime)((OracleTimeStamp)outParam).Value;
                else
                    return null;
            }
            else if (t == typeof(OracleBlob))
            {
                if (!((OracleBlob)outParam).IsNull)
                    return (byte[])((OracleBlob)outParam).Value;
                else
                    return null;
            }
            return outParam;
        }
        #endregion

    }
    public partial class DAOUserRoles : OraSync_BaseData
    {
        #region member variables
        protected Int64? _nb;
        protected Int64? _rolenb;
        protected Int64? _usernb;
        protected Int64? _ordr;
        #endregion

        #region class methods
        public DAOUserRoles()
        {
        }
        ///<Summary>
        ///Select one row by primary key(s)
        ///This method returns one row from the table USER_ROLES based on the primary key(s)
        ///</Summary>
        ///<returns>
        ///DAOAppmgrUserRoles
        ///</returns>
        ///<parameters>
        ///Int64? nb
        ///</parameters>
        public static DAOUserRoles SelectOne(Int64? nb)
        {
            OracleCommand command = new OracleCommand();
            command.CommandText = "APPMGR.PUSER_ROLES.CTPR_GETONE";
            command.CommandType = CommandType.StoredProcedure;
            OracleConnection staticConnection = StaticSqlConnection;
            command.Connection = staticConnection;

            DataTable dt = new DataTable("USER_ROLES");
            OracleDataAdapter sqlAdapter = new OracleDataAdapter(command);
            try
            {
                command.Parameters.Add(new OracleParameter("P_NB", OracleDbType.Int64, 22, ParameterDirection.Input, false, 10, 0, "", DataRowVersion.Proposed, (object)nb ?? (object)DBNull.Value));

                OracleParameter refCursor = new OracleParameter("RESULTS", OracleDbType.RefCursor);
                refCursor.Direction = ParameterDirection.Output;
                command.Parameters.Add(refCursor);

                staticConnection.Open();
                sqlAdapter.Fill(dt);


                DAOUserRoles retObj = null;
                if (dt.Rows.Count > 0)
                {
                    retObj = new DAOUserRoles();
                    retObj._nb = Convert.IsDBNull(dt.Rows[0]["NB"]) ? (Int64?)null : (Int64?)dt.Rows[0]["NB"];
                    retObj._rolenb = Convert.IsDBNull(dt.Rows[0]["ROLENB"]) ? (Int64?)null : (Int64?)dt.Rows[0]["ROLENB"];
                    retObj._usernb = Convert.IsDBNull(dt.Rows[0]["USERNB"]) ? (Int64?)null : (Int64?)dt.Rows[0]["USERNB"];
                    retObj._ordr = Convert.IsDBNull(dt.Rows[0]["ORDR"]) ? (Int64?)null : (Int64?)dt.Rows[0]["ORDR"];
                }
                return retObj;
            }
            catch (Exception ex)
            {
                throw new Exception("DAOAppmgrUserRoles::Error Occurred:SelectOne:" + ex.Message, ex);
            }
            finally
            {
                staticConnection.Close();
                command.Dispose();
            }
        }

        ///<Summary>
        ///Delete one row by primary key(s)
        ///this method allows the object to delete itself from the table USER_ROLES based on its primary key
        ///</Summary>
        ///<returns>
        ///void
        ///</returns>
        ///<parameters>
        ///
        ///</parameters>
        public virtual void Delete()
        {
            OracleCommand command = new OracleCommand();
            command.CommandText = "APPMGR.PUSER_ROLES.CTPR_DELETEONE";
            command.CommandType = CommandType.StoredProcedure;
            command.Connection = _connectionProvider.Connection;

            try
            {
                command.Parameters.Add(new OracleParameter("P_NB", OracleDbType.Int64, 22, ParameterDirection.Input, false, 10, 0, "", DataRowVersion.Proposed, (object)_nb ?? (object)DBNull.Value));

                command.ExecuteNonQuery();


            }
            catch (Exception ex)
            {
                throw new Exception("DAOAppmgrUserRoles::Error Occurred:Delete:" + ex.Message, ex);
            }
            finally
            {
                command.Dispose();
            }
        }

        ///<Summary>
        ///Select all rows by foreign key
        ///This method returns all data rows in the table USER_ROLES based on a foreign key
        ///</Summary>
        ///<returns>
        ///List-DAOAppmgrUserRoles.
        ///</returns>
        ///<parameters>
        ///Int64? rolenb
        ///</parameters>
        public static List<DAOUserRoles> SelectAllByRolenb(Int64? rolenb)
        {
            OracleCommand command = new OracleCommand();
            command.CommandText = "APPMGR.PUSER_ROLES.CTPR_GETBYROLENB";
            command.CommandType = CommandType.StoredProcedure;
            OracleConnection staticConnection = StaticSqlConnection;
            command.Connection = staticConnection;

            DataTable dt = new DataTable("USER_ROLES");
            OracleDataAdapter sqlAdapter = new OracleDataAdapter(command);
            try
            {
                command.Parameters.Add(new OracleParameter("P_ROLENB", OracleDbType.Int64, 22, ParameterDirection.Input, false, 10, 0, "", DataRowVersion.Proposed, (object)rolenb ?? (object)DBNull.Value));

                OracleParameter refCursor = new OracleParameter("RESULTS", OracleDbType.RefCursor);
                refCursor.Direction = ParameterDirection.Output;
                command.Parameters.Add(refCursor);

                staticConnection.Open();
                sqlAdapter.Fill(dt);


                List<DAOUserRoles> objList = new List<DAOUserRoles>();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        DAOUserRoles retObj = new DAOUserRoles();
                        retObj._nb = Convert.IsDBNull(row["NB"]) ? (Int64?)null : (Int64?)row["NB"];
                        retObj._rolenb = Convert.IsDBNull(row["ROLENB"]) ? (Int64?)null : (Int64?)row["ROLENB"];
                        retObj._usernb = Convert.IsDBNull(row["USERNB"]) ? (Int64?)null : (Int64?)row["USERNB"];
                        retObj._ordr = Convert.IsDBNull(row["ORDR"]) ? (Int64?)null : (Int64?)row["ORDR"];
                        objList.Add(retObj);
                    }
                }
                return objList;
            }
            catch (Exception ex)
            {
                throw new Exception("DAOAppmgrUserRoles::Error Occurred:SelectAllByRolenb:" + ex.Message, ex);
            }
            finally
            {
                staticConnection.Close();
                command.Dispose();
            }
        }

        ///<Summary>
        ///</Summary>
        ///<returns>
        ///Int32
        ///</returns>
        ///<parameters>
        ///Int64? rolenb
        ///</parameters>
        public static Int32 SelectAllByRolenbCount(Int64? rolenb)
        {
            OracleCommand command = new OracleCommand();
            command.CommandText = "APPMGR.PUSER_ROLES.CTPR_GETBYROLENBCOUNT";
            command.CommandType = CommandType.StoredProcedure;
            OracleConnection staticConnection = StaticSqlConnection;
            command.Connection = staticConnection;

            try
            {
                command.Parameters.Add(new OracleParameter("P_ROLENB", OracleDbType.Int64, 22, ParameterDirection.Input, false, 10, 0, "", DataRowVersion.Proposed, (object)rolenb ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_COUNT", OracleDbType.Int32, 4, ParameterDirection.Output, false, 10, 0, "", DataRowVersion.Proposed, null));

                staticConnection.Open();
                command.ExecuteNonQuery();

                Int32 retCount = (Int32)(OracleDecimal)command.Parameters["P_COUNT"].Value;

                return retCount;
            }
            catch (Exception ex)
            {
                throw new Exception("DAOAppmgrUserRoles::Error Occurred:SelectAllByRolenbCount:" + ex.Message, ex);
            }
            finally
            {
                staticConnection.Close();
                command.Dispose();
            }
        }

        ///<Summary>
        ///This method deletes all rows in the table USER_ROLES with a given foreign key
        ///This method deletes all rows in the table USER_ROLES with a given foreign key
        ///</Summary>
        ///<returns>
        ///void
        ///</returns>
        ///<parameters>
        ///TxConnectionProvider connectionProvider, Int64? rolenb
        ///</parameters>
        public static void DeleteAllByRolenb(OraSync_TxConnectionProvider connectionProvider, Int64? rolenb)
        {
            OracleCommand command = new OracleCommand();
            command.CommandText = "APPMGR.PUSER_ROLES.CTPR_DELETEBYROLENB";
            command.CommandType = CommandType.StoredProcedure;
            command.Connection = connectionProvider.Connection;
            command.Transaction = connectionProvider.CurrentTransaction;

            try
            {
                command.Parameters.Add(new OracleParameter("P_ROLENB", OracleDbType.Int64, 22, ParameterDirection.Input, false, 10, 0, "", DataRowVersion.Proposed, (object)rolenb ?? (object)DBNull.Value));

                command.ExecuteNonQuery();


            }
            catch (Exception ex)
            {
                throw new Exception("DAOAppmgrUserRoles::Error Occurred:DeleteAllByRolenb:" + ex.Message, ex);
            }
            finally
            {
                command.Dispose();
            }
        }

        ///<Summary>
        ///Select all rows by foreign key
        ///This method returns all data rows in the table USER_ROLES based on a foreign key
        ///</Summary>
        ///<returns>
        ///List-DAOAppmgrUserRoles.
        ///</returns>
        ///<parameters>
        ///Int64? usernb
        ///</parameters>
        public static List<DAOUserRoles> SelectAllByUsernb(Int64? usernb)
        {
            OracleCommand command = new OracleCommand();
            command.CommandText = "APPMGR.PUSER_ROLES.CTPR_GETBYUSERNB";
            command.CommandType = CommandType.StoredProcedure;
            OracleConnection staticConnection = StaticSqlConnection;
            command.Connection = staticConnection;

            DataTable dt = new DataTable("USER_ROLES");
            OracleDataAdapter sqlAdapter = new OracleDataAdapter(command);
            try
            {
                command.Parameters.Add(new OracleParameter("P_USERNB", OracleDbType.Int64, 22, ParameterDirection.Input, false, 10, 0, "", DataRowVersion.Proposed, (object)usernb ?? (object)DBNull.Value));

                OracleParameter refCursor = new OracleParameter("RESULTS", OracleDbType.RefCursor);
                refCursor.Direction = ParameterDirection.Output;
                command.Parameters.Add(refCursor);

                staticConnection.Open();
                sqlAdapter.Fill(dt);


                List<DAOUserRoles> objList = new List<DAOUserRoles>();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        DAOUserRoles retObj = new DAOUserRoles();
                        retObj._nb = Convert.IsDBNull(row["NB"]) ? (Int64?)null : (Int64?)row["NB"];
                        retObj._rolenb = Convert.IsDBNull(row["ROLENB"]) ? (Int64?)null : (Int64?)row["ROLENB"];
                        retObj._usernb = Convert.IsDBNull(row["USERNB"]) ? (Int64?)null : (Int64?)row["USERNB"];
                        retObj._ordr = Convert.IsDBNull(row["ORDR"]) ? (Int64?)null : (Int64?)row["ORDR"];
                        objList.Add(retObj);
                    }
                }
                return objList;
            }
            catch (Exception ex)
            {
                throw new Exception("DAOAppmgrUserRoles::Error Occurred:SelectAllByUsernb:" + ex.Message, ex);
            }
            finally
            {
                staticConnection.Close();
                command.Dispose();
            }
        }

        ///<Summary>
        ///</Summary>
        ///<returns>
        ///Int32
        ///</returns>
        ///<parameters>
        ///Int64? usernb
        ///</parameters>
        public static Int32 SelectAllByUsernbCount(Int64? usernb)
        {
            OracleCommand command = new OracleCommand();
            command.CommandText = "APPMGR.PUSER_ROLES.CTPR_GETBYUSERNBCOUNT";
            command.CommandType = CommandType.StoredProcedure;
            OracleConnection staticConnection = StaticSqlConnection;
            command.Connection = staticConnection;

            try
            {
                command.Parameters.Add(new OracleParameter("P_USERNB", OracleDbType.Int64, 22, ParameterDirection.Input, false, 10, 0, "", DataRowVersion.Proposed, (object)usernb ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_COUNT", OracleDbType.Int32, 4, ParameterDirection.Output, false, 10, 0, "", DataRowVersion.Proposed, null));

                staticConnection.Open();
                command.ExecuteNonQuery();

                Int32 retCount = (Int32)(OracleDecimal)command.Parameters["P_COUNT"].Value;

                return retCount;
            }
            catch (Exception ex)
            {
                throw new Exception("DAOAppmgrUserRoles::Error Occurred:SelectAllByUsernbCount:" + ex.Message, ex);
            }
            finally
            {
                staticConnection.Close();
                command.Dispose();
            }
        }

        ///<Summary>
        ///This method deletes all rows in the table USER_ROLES with a given foreign key
        ///This method deletes all rows in the table USER_ROLES with a given foreign key
        ///</Summary>
        ///<returns>
        ///void
        ///</returns>
        ///<parameters>
        ///TxConnectionProvider connectionProvider, Int64? usernb
        ///</parameters>
        public static void DeleteAllByUsernb(OraSync_TxConnectionProvider connectionProvider, Int64? usernb)
        {
            OracleCommand command = new OracleCommand();
            command.CommandText = "APPMGR.PUSER_ROLES.CTPR_DELETEBYUSERNB";
            command.CommandType = CommandType.StoredProcedure;
            command.Connection = connectionProvider.Connection;
            command.Transaction = connectionProvider.CurrentTransaction;

            try
            {
                command.Parameters.Add(new OracleParameter("P_USERNB", OracleDbType.Int64, 22, ParameterDirection.Input, false, 10, 0, "", DataRowVersion.Proposed, (object)usernb ?? (object)DBNull.Value));

                command.ExecuteNonQuery();


            }
            catch (Exception ex)
            {
                throw new Exception("DAOAppmgrUserRoles::Error Occurred:DeleteAllByUsernb:" + ex.Message, ex);
            }
            finally
            {
                command.Dispose();
            }
        }

        ///<Summary>
        ///Insert a new row
        ///This method saves a new object to the table USER_ROLES
        ///</Summary>
        ///<returns>
        ///void
        ///</returns>
        ///<parameters>
        ///
        ///</parameters>
        public virtual void Insert()
        {
            OracleCommand command = new OracleCommand();
            command.CommandText = "APPMGR.PUSER_ROLES.CTPR_INSERTONE";
            command.CommandType = CommandType.StoredProcedure;
            command.Connection = _connectionProvider.Connection;

            try
            {
                command.Parameters.Add(new OracleParameter("P_NB", OracleDbType.Int64, 22, ParameterDirection.InputOutput, true, 10, 0, "", DataRowVersion.Proposed, _nb));
                command.Parameters.Add(new OracleParameter("P_ROLENB", OracleDbType.Int64, 22, ParameterDirection.InputOutput, true, 10, 0, "", DataRowVersion.Proposed, _rolenb));
                command.Parameters.Add(new OracleParameter("P_USERNB", OracleDbType.Int64, 22, ParameterDirection.InputOutput, true, 10, 0, "", DataRowVersion.Proposed, _usernb));
                command.Parameters.Add(new OracleParameter("P_ORDR", OracleDbType.Int64, 22, ParameterDirection.InputOutput, true, 10, 0, "", DataRowVersion.Proposed, _ordr));

                command.ExecuteNonQuery();

                _nb = (Int64?)(decimal?)UnboxOracleParam(command.Parameters["P_NB"].Value);
                _rolenb = (Int64?)(decimal?)UnboxOracleParam(command.Parameters["P_ROLENB"].Value);
                _usernb = (Int64?)(decimal?)UnboxOracleParam(command.Parameters["P_USERNB"].Value);
                _ordr = (Int64?)(decimal?)UnboxOracleParam(command.Parameters["P_ORDR"].Value);

            }
            catch (Exception ex)
            {
                throw new Exception("DAOAppmgrUserRoles::Error Occurred:Insert:" + ex.Message, ex);
            }
            finally
            {
                command.Dispose();
            }
        }

        ///<Summary>
        ///Select all rows
        ///This method returns all data rows in the table USER_ROLES
        ///</Summary>
        ///<returns>
        ///List-DAOAppmgrUserRoles.
        ///</returns>
        ///<parameters>
        ///
        ///</parameters>
        public static List<DAOUserRoles> SelectAll()
        {
            OracleCommand command = new OracleCommand();
            command.CommandText = "APPMGR.PUSER_ROLES.CTPR_GETALL";
            command.CommandType = CommandType.StoredProcedure;
            OracleConnection staticConnection = StaticSqlConnection;
            command.Connection = staticConnection;

            DataTable dt = new DataTable("USER_ROLES");
            OracleDataAdapter sqlAdapter = new OracleDataAdapter(command);
            try
            {

                OracleParameter refCursor = new OracleParameter("RESULTS", OracleDbType.RefCursor);
                refCursor.Direction = ParameterDirection.Output;
                command.Parameters.Add(refCursor);

                staticConnection.Open();
                sqlAdapter.Fill(dt);


                List<DAOUserRoles> objList = new List<DAOUserRoles>();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        DAOUserRoles retObj = new DAOUserRoles();
                        retObj._nb = Convert.IsDBNull(row["NB"]) ? (Int64?)null : (Int64?)row["NB"];
                        retObj._rolenb = Convert.IsDBNull(row["ROLENB"]) ? (Int64?)null : (Int64?)row["ROLENB"];
                        retObj._usernb = Convert.IsDBNull(row["USERNB"]) ? (Int64?)null : (Int64?)row["USERNB"];
                        retObj._ordr = Convert.IsDBNull(row["ORDR"]) ? (Int64?)null : (Int64?)row["ORDR"];
                        objList.Add(retObj);
                    }
                }
                return objList;
            }
            catch (Exception ex)
            {
                throw new Exception("DAOAppmgrUserRoles::Error Occurred:SelectAll:" + ex.Message, ex);
            }
            finally
            {
                staticConnection.Close();
                command.Dispose();
            }
        }

        ///<Summary>
        ///</Summary>
        ///<returns>
        ///Int32
        ///</returns>
        ///<parameters>
        ///
        ///</parameters>
        public static Int32 SelectAllCount()
        {
            OracleCommand command = new OracleCommand();
            command.CommandText = "APPMGR.PUSER_ROLES.CTPR_GETALLCOUNT";
            command.CommandType = CommandType.StoredProcedure;
            OracleConnection staticConnection = StaticSqlConnection;
            command.Connection = staticConnection;

            try
            {
                command.Parameters.Add(new OracleParameter("P_COUNT", OracleDbType.Int32, 4, ParameterDirection.Output, false, 10, 0, "", DataRowVersion.Proposed, null));

                staticConnection.Open();
                command.ExecuteNonQuery();

                Int32 retCount = (Int32)(OracleDecimal)command.Parameters["P_COUNT"].Value;

                return retCount;
            }
            catch (Exception ex)
            {
                throw new Exception("DAOAppmgrUserRoles::Error Occurred:SelectAllCount:" + ex.Message, ex);
            }
            finally
            {
                staticConnection.Close();
                command.Dispose();
            }
        }

        ///<Summary>
        ///</Summary>
        ///<returns>
        ///List-DAOAppmgrUserRoles.
        ///</returns>
        ///<parameters>
        ///DAOAppmgrUserRoles daoAppmgrUserRoles
        ///</parameters>
        public static List<DAOUserRoles> SelectAllBySearchFields(DAOUserRoles daoAppmgrUserRoles)
        {
            OracleCommand command = new OracleCommand();
            command.CommandText = "APPMGR.PUSER_ROLES.CTPR_GETBYSEARCHFIELDS";
            command.CommandType = CommandType.StoredProcedure;
            OracleConnection staticConnection = StaticSqlConnection;
            command.Connection = staticConnection;

            DataTable dt = new DataTable("USER_ROLES");
            OracleDataAdapter sqlAdapter = new OracleDataAdapter(command);
            try
            {
                command.Parameters.Add(new OracleParameter("P_NB", OracleDbType.Int64, 22, ParameterDirection.Input, false, 10, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUserRoles.Nb ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_ROLENB", OracleDbType.Int64, 22, ParameterDirection.Input, false, 10, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUserRoles.Rolenb ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_USERNB", OracleDbType.Int64, 22, ParameterDirection.Input, false, 10, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUserRoles.Usernb ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_ORDR", OracleDbType.Int64, 22, ParameterDirection.Input, true, 10, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUserRoles.Ordr ?? (object)DBNull.Value));

                OracleParameter refCursor = new OracleParameter("RESULTS", OracleDbType.RefCursor);
                refCursor.Direction = ParameterDirection.Output;
                command.Parameters.Add(refCursor);

                staticConnection.Open();
                sqlAdapter.Fill(dt);


                List<DAOUserRoles> objList = new List<DAOUserRoles>();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        DAOUserRoles retObj = new DAOUserRoles();
                        retObj._nb = Convert.IsDBNull(row["NB"]) ? (Int64?)null : (Int64?)row["NB"];
                        retObj._rolenb = Convert.IsDBNull(row["ROLENB"]) ? (Int64?)null : (Int64?)row["ROLENB"];
                        retObj._usernb = Convert.IsDBNull(row["USERNB"]) ? (Int64?)null : (Int64?)row["USERNB"];
                        retObj._ordr = Convert.IsDBNull(row["ORDR"]) ? (Int64?)null : (Int64?)row["ORDR"];
                        objList.Add(retObj);
                    }
                }
                return objList;
            }
            catch (Exception ex)
            {
                throw new Exception("DAOAppmgrUserRoles::Error Occurred:SelectAllBySearchFields:" + ex.Message, ex);
            }
            finally
            {
                staticConnection.Close();
                command.Dispose();
            }
        }

        ///<Summary>
        ///</Summary>
        ///<returns>
        ///Int32
        ///</returns>
        ///<parameters>
        ///DAOAppmgrUserRoles daoAppmgrUserRoles
        ///</parameters>
        public static Int32 SelectAllBySearchFieldsCount(DAOUserRoles daoAppmgrUserRoles)
        {
            OracleCommand command = new OracleCommand();
            command.CommandText = "APPMGR.PUSER_ROLES.CTPR_GETBYSEARCHFIELDSCOUNT";
            command.CommandType = CommandType.StoredProcedure;
            OracleConnection staticConnection = StaticSqlConnection;
            command.Connection = staticConnection;

            try
            {
                command.Parameters.Add(new OracleParameter("P_NB", OracleDbType.Int64, 22, ParameterDirection.Input, false, 10, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUserRoles.Nb ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_ROLENB", OracleDbType.Int64, 22, ParameterDirection.Input, false, 10, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUserRoles.Rolenb ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_USERNB", OracleDbType.Int64, 22, ParameterDirection.Input, false, 10, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUserRoles.Usernb ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_ORDR", OracleDbType.Int64, 22, ParameterDirection.Input, true, 10, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUserRoles.Ordr ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_COUNT", OracleDbType.Int32, 4, ParameterDirection.Output, false, 10, 0, "", DataRowVersion.Proposed, null));

                staticConnection.Open();
                command.ExecuteNonQuery();

                Int32 retCount = (Int32)(OracleDecimal)command.Parameters["P_COUNT"].Value;

                return retCount;
            }
            catch (Exception ex)
            {
                throw new Exception("DAOAppmgrUserRoles::Error Occurred:SelectAllBySearchFieldsCount:" + ex.Message, ex);
            }
            finally
            {
                staticConnection.Close();
                command.Dispose();
            }
        }

        ///<Summary>
        ///Update one row by primary key(s)
        ///This method allows the object to update itself in the table USER_ROLES based on its primary key(s)
        ///</Summary>
        ///<returns>
        ///void
        ///</returns>
        ///<parameters>
        ///
        ///</parameters>
        public virtual void Update()
        {
            OracleCommand command = new OracleCommand();
            command.CommandText = "APPMGR.PUSER_ROLES.CTPR_UPDATEONE";
            command.CommandType = CommandType.StoredProcedure;
            command.Connection = _connectionProvider.Connection;

            try
            {
                command.Parameters.Add(new OracleParameter("P_NB", OracleDbType.Int64, 22, ParameterDirection.InputOutput, true, 10, 0, "", DataRowVersion.Proposed, _nb));
                command.Parameters.Add(new OracleParameter("P_ROLENB", OracleDbType.Int64, 22, ParameterDirection.InputOutput, true, 10, 0, "", DataRowVersion.Proposed, _rolenb));
                command.Parameters.Add(new OracleParameter("P_USERNB", OracleDbType.Int64, 22, ParameterDirection.InputOutput, true, 10, 0, "", DataRowVersion.Proposed, _usernb));
                command.Parameters.Add(new OracleParameter("P_ORDR", OracleDbType.Int64, 22, ParameterDirection.InputOutput, true, 10, 0, "", DataRowVersion.Proposed, _ordr));

                command.ExecuteNonQuery();

                _nb = (Int64?)(decimal?)UnboxOracleParam(command.Parameters["P_NB"].Value);
                _rolenb = (Int64?)(decimal?)UnboxOracleParam(command.Parameters["P_ROLENB"].Value);
                _usernb = (Int64?)(decimal?)UnboxOracleParam(command.Parameters["P_USERNB"].Value);
                _ordr = (Int64?)(decimal?)UnboxOracleParam(command.Parameters["P_ORDR"].Value);

            }
            catch (Exception ex)
            {
                throw new Exception("DAOAppmgrUserRoles::Error Occurred:Update:" + ex.Message, ex);
            }
            finally
            {
                command.Dispose();
            }
        }

        #endregion

        #region member properties
        public Int64? Nb
        {
            get
            {
                return _nb;
            }
            set
            {
                _nb = value;
            }
        }
        public Int64? Rolenb
        {
            get
            {
                return _rolenb;
            }
            set
            {
                _rolenb = value;
            }
        }
        public Int64? Usernb
        {
            get
            {
                return _usernb;
            }
            set
            {
                _usernb = value;
            }
        }
        public Int64? Ordr
        {
            get
            {
                return _ordr;
            }
            set
            {
                _ordr = value;
            }
        }
        #endregion
    }
    public partial class DAOUsers : OraSync_BaseData
    {
        #region member variables
        protected Int64? _nb;
        protected string _username;
        protected string _afirstname;
        protected string _alastname;
        protected string _password;
        protected DateTime? _created;
        protected string _gendr;
        protected string _locked;
        protected Int64? _deptnb;
        protected string _prefix;
        protected Int64? _visible;
        protected Int64? _ssystem;
        protected Int16? _securlevel;
        protected Int64? _canchpasswd;
        protected Int64? _mustchpasswd;
        protected Int64? _sessionco;
        protected Int64? _carsesco;
        protected Int64? _idltim;
        protected string _passwordBond;
        protected string _mechanicPass;
        protected string _insurancePass;
        protected DateTime? _idate;
        protected string _iuser;
        protected DateTime? _udate;
        protected string _uuser;
        protected string _empid;
        protected string _mailcode;
        protected decimal? _position;
        protected Int64? _ctynb;
        #endregion

        #region class methods
        public DAOUsers()
        {
        }
        ///<Summary>
        ///Select one row by primary key(s)
        ///This method returns one row from the table USERS based on the primary key(s)
        ///</Summary>
        ///<returns>
        ///DAOAppmgrUsers
        ///</returns>
        ///<parameters>
        ///Int64? nb
        ///</parameters>
        public static DAOUsers SelectOne(Int64? nb)
        {
            OracleCommand command = new OracleCommand();
            command.CommandText = "APPMGR.PUSERS.CTPR_GETONE";
            command.CommandType = CommandType.StoredProcedure;
            OracleConnection staticConnection = StaticSqlConnection;
            command.Connection = staticConnection;

            DataTable dt = new DataTable("USERS");
            OracleDataAdapter sqlAdapter = new OracleDataAdapter(command);
            try
            {
                command.Parameters.Add(new OracleParameter("P_NB", OracleDbType.Int64, 22, ParameterDirection.Input, false, 10, 0, "", DataRowVersion.Proposed, (object)nb ?? (object)DBNull.Value));

                OracleParameter refCursor = new OracleParameter("RESULTS", OracleDbType.RefCursor);
                refCursor.Direction = ParameterDirection.Output;
                command.Parameters.Add(refCursor);

                staticConnection.Open();
                sqlAdapter.Fill(dt);


                DAOUsers retObj = null;
                if (dt.Rows.Count > 0)
                {
                    retObj = new DAOUsers();
                    retObj._nb = Convert.IsDBNull(dt.Rows[0]["NB"]) ? (Int64?)null : (Int64?)dt.Rows[0]["NB"];
                    retObj._username = Convert.IsDBNull(dt.Rows[0]["USERNAME"]) ? null : (string)dt.Rows[0]["USERNAME"];
                    retObj._afirstname = Convert.IsDBNull(dt.Rows[0]["AFIRSTNAME"]) ? null : (string)dt.Rows[0]["AFIRSTNAME"];
                    retObj._alastname = Convert.IsDBNull(dt.Rows[0]["ALASTNAME"]) ? null : (string)dt.Rows[0]["ALASTNAME"];
                    retObj._password = Convert.IsDBNull(dt.Rows[0]["PASSWORD"]) ? null : (string)dt.Rows[0]["PASSWORD"];
                    retObj._created = Convert.IsDBNull(dt.Rows[0]["CREATED"]) ? (DateTime?)null : (DateTime?)dt.Rows[0]["CREATED"];
                    retObj._gendr = Convert.IsDBNull(dt.Rows[0]["GENDR"]) ? null : (string)dt.Rows[0]["GENDR"];
                    retObj._locked = Convert.IsDBNull(dt.Rows[0]["LOCKED"]) ? null : (string)dt.Rows[0]["LOCKED"];
                    retObj._deptnb = Convert.IsDBNull(dt.Rows[0]["DEPTNB"]) ? (Int64?)null : (Int64?)dt.Rows[0]["DEPTNB"];
                    retObj._prefix = Convert.IsDBNull(dt.Rows[0]["PREFIX"]) ? null : (string)dt.Rows[0]["PREFIX"];
                    retObj._visible = Convert.IsDBNull(dt.Rows[0]["VISIBLE"]) ? (Int64?)null : (Int64?)dt.Rows[0]["VISIBLE"];
                    retObj._ssystem = Convert.IsDBNull(dt.Rows[0]["SSYSTEM"]) ? (Int64?)null : (Int64?)dt.Rows[0]["SSYSTEM"];
                    retObj._securlevel = Convert.IsDBNull(dt.Rows[0]["SECURLEVEL"]) ? (Int16?)null : (Int16?)dt.Rows[0]["SECURLEVEL"];
                    retObj._canchpasswd = Convert.IsDBNull(dt.Rows[0]["CANCHPASSWD"]) ? (Int64?)null : (Int64?)dt.Rows[0]["CANCHPASSWD"];
                    retObj._mustchpasswd = Convert.IsDBNull(dt.Rows[0]["MUSTCHPASSWD"]) ? (Int64?)null : (Int64?)dt.Rows[0]["MUSTCHPASSWD"];
                    retObj._sessionco = Convert.IsDBNull(dt.Rows[0]["SESSIONCO"]) ? (Int64?)null : (Int64?)dt.Rows[0]["SESSIONCO"];
                    retObj._carsesco = Convert.IsDBNull(dt.Rows[0]["CARSESCO"]) ? (Int64?)null : (Int64?)dt.Rows[0]["CARSESCO"];
                    retObj._idltim = Convert.IsDBNull(dt.Rows[0]["IDLTIM"]) ? (Int64?)null : (Int64?)dt.Rows[0]["IDLTIM"];
                    retObj._passwordBond = Convert.IsDBNull(dt.Rows[0]["PASSWORD_BOND"]) ? null : (string)dt.Rows[0]["PASSWORD_BOND"];
                    retObj._mechanicPass = Convert.IsDBNull(dt.Rows[0]["MECHANIC_PASS"]) ? null : (string)dt.Rows[0]["MECHANIC_PASS"];
                    retObj._insurancePass = Convert.IsDBNull(dt.Rows[0]["INSURANCE_PASS"]) ? null : (string)dt.Rows[0]["INSURANCE_PASS"];
                    retObj._idate = Convert.IsDBNull(dt.Rows[0]["IDATE"]) ? (DateTime?)null : (DateTime?)dt.Rows[0]["IDATE"];
                    retObj._iuser = Convert.IsDBNull(dt.Rows[0]["IUSER"]) ? null : (string)dt.Rows[0]["IUSER"];
                    retObj._udate = Convert.IsDBNull(dt.Rows[0]["UDATE"]) ? (DateTime?)null : (DateTime?)dt.Rows[0]["UDATE"];
                    retObj._uuser = Convert.IsDBNull(dt.Rows[0]["UUSER"]) ? null : (string)dt.Rows[0]["UUSER"];
                    retObj._empid = Convert.IsDBNull(dt.Rows[0]["EMPID"]) ? null : (string)dt.Rows[0]["EMPID"];
                    retObj._mailcode = Convert.IsDBNull(dt.Rows[0]["MAILCODE"]) ? null : (string)dt.Rows[0]["MAILCODE"];
                    retObj._position = Convert.IsDBNull(dt.Rows[0]["POSITION"]) ? (decimal?)null : (decimal?)dt.Rows[0]["POSITION"];
                    retObj._ctynb = Convert.IsDBNull(dt.Rows[0]["CTY_NB"]) ? (Int64?)null : (Int64?)dt.Rows[0]["CTY_NB"];
                }
                return retObj;
            }
            catch (Exception ex)
            {
                throw new Exception("DAOAppmgrUsers::Error Occurred:SelectOne:" + ex.Message, ex);
            }
            finally
            {
                staticConnection.Close();
                command.Dispose();
            }
        }

        ///<Summary>
        ///Delete one row by primary key(s)
        ///this method allows the object to delete itself from the table USERS based on its primary key
        ///</Summary>
        ///<returns>
        ///void
        ///</returns>
        ///<parameters>
        ///
        ///</parameters>
        public virtual void Delete()
        {
            OracleCommand command = new OracleCommand();
            command.CommandText = "APPMGR.PUSERS.CTPR_DELETEONE";
            command.CommandType = CommandType.StoredProcedure;
            command.Connection = _connectionProvider.Connection;

            try
            {
                command.Parameters.Add(new OracleParameter("P_NB", OracleDbType.Int64, 22, ParameterDirection.Input, false, 10, 0, "", DataRowVersion.Proposed, (object)_nb ?? (object)DBNull.Value));

                command.ExecuteNonQuery();


            }
            catch (Exception ex)
            {
                throw new Exception("DAOAppmgrUsers::Error Occurred:Delete:" + ex.Message, ex);
            }
            finally
            {
                command.Dispose();
            }
        }

        ///<Summary>
        ///Select one row by unique constraint
        ///This method returns one row from the table USERS based on a unique constraint
        ///</Summary>
        ///<returns>
        ///DAOAppmgrUsers
        ///</returns>
        ///<parameters>
        ///string username
        ///</parameters>
        public static DAOUsers SelectOneByUsername(string username)
        {
            OracleCommand command = new OracleCommand();
            command.CommandText = "APPMGR.PUSERS.CTPR_GETONEBYUSERNAME";
            command.CommandType = CommandType.StoredProcedure;
            OracleConnection staticConnection = StaticSqlConnection;
            command.Connection = staticConnection;

            DataTable dt = new DataTable("USERS");
            OracleDataAdapter sqlAdapter = new OracleDataAdapter(command);
            try
            {
                command.Parameters.Add(new OracleParameter("P_USERNAME", OracleDbType.Varchar2, 40, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Proposed, (object)username ?? (object)DBNull.Value));

                OracleParameter refCursor = new OracleParameter("RESULTS", OracleDbType.RefCursor);
                refCursor.Direction = ParameterDirection.Output;
                command.Parameters.Add(refCursor);

                staticConnection.Open();
                sqlAdapter.Fill(dt);


                DAOUsers retObj = null;
                if (dt.Rows.Count > 0)
                {
                    retObj = new DAOUsers();
                    retObj._nb = Convert.IsDBNull(dt.Rows[0]["NB"]) ? (Int64?)null : (Int64?)dt.Rows[0]["NB"];
                    retObj._username = Convert.IsDBNull(dt.Rows[0]["USERNAME"]) ? null : (string)dt.Rows[0]["USERNAME"];
                    retObj._afirstname = Convert.IsDBNull(dt.Rows[0]["AFIRSTNAME"]) ? null : (string)dt.Rows[0]["AFIRSTNAME"];
                    retObj._alastname = Convert.IsDBNull(dt.Rows[0]["ALASTNAME"]) ? null : (string)dt.Rows[0]["ALASTNAME"];
                    retObj._password = Convert.IsDBNull(dt.Rows[0]["PASSWORD"]) ? null : (string)dt.Rows[0]["PASSWORD"];
                    retObj._created = Convert.IsDBNull(dt.Rows[0]["CREATED"]) ? (DateTime?)null : (DateTime?)dt.Rows[0]["CREATED"];
                    retObj._gendr = Convert.IsDBNull(dt.Rows[0]["GENDR"]) ? null : (string)dt.Rows[0]["GENDR"];
                    retObj._locked = Convert.IsDBNull(dt.Rows[0]["LOCKED"]) ? null : (string)dt.Rows[0]["LOCKED"];
                    retObj._deptnb = Convert.IsDBNull(dt.Rows[0]["DEPTNB"]) ? (Int64?)null : (Int64?)dt.Rows[0]["DEPTNB"];
                    retObj._prefix = Convert.IsDBNull(dt.Rows[0]["PREFIX"]) ? null : (string)dt.Rows[0]["PREFIX"];
                    retObj._visible = Convert.IsDBNull(dt.Rows[0]["VISIBLE"]) ? (Int64?)null : (Int64?)dt.Rows[0]["VISIBLE"];
                    retObj._ssystem = Convert.IsDBNull(dt.Rows[0]["SSYSTEM"]) ? (Int64?)null : (Int64?)dt.Rows[0]["SSYSTEM"];
                    retObj._securlevel = Convert.IsDBNull(dt.Rows[0]["SECURLEVEL"]) ? (Int16?)null : (Int16?)dt.Rows[0]["SECURLEVEL"];
                    retObj._canchpasswd = Convert.IsDBNull(dt.Rows[0]["CANCHPASSWD"]) ? (Int64?)null : (Int64?)dt.Rows[0]["CANCHPASSWD"];
                    retObj._mustchpasswd = Convert.IsDBNull(dt.Rows[0]["MUSTCHPASSWD"]) ? (Int64?)null : (Int64?)dt.Rows[0]["MUSTCHPASSWD"];
                    retObj._sessionco = Convert.IsDBNull(dt.Rows[0]["SESSIONCO"]) ? (Int64?)null : (Int64?)dt.Rows[0]["SESSIONCO"];
                    retObj._carsesco = Convert.IsDBNull(dt.Rows[0]["CARSESCO"]) ? (Int64?)null : (Int64?)dt.Rows[0]["CARSESCO"];
                    retObj._idltim = Convert.IsDBNull(dt.Rows[0]["IDLTIM"]) ? (Int64?)null : (Int64?)dt.Rows[0]["IDLTIM"];
                    retObj._passwordBond = Convert.IsDBNull(dt.Rows[0]["PASSWORD_BOND"]) ? null : (string)dt.Rows[0]["PASSWORD_BOND"];
                    retObj._mechanicPass = Convert.IsDBNull(dt.Rows[0]["MECHANIC_PASS"]) ? null : (string)dt.Rows[0]["MECHANIC_PASS"];
                    retObj._insurancePass = Convert.IsDBNull(dt.Rows[0]["INSURANCE_PASS"]) ? null : (string)dt.Rows[0]["INSURANCE_PASS"];
                    retObj._idate = Convert.IsDBNull(dt.Rows[0]["IDATE"]) ? (DateTime?)null : (DateTime?)dt.Rows[0]["IDATE"];
                    retObj._iuser = Convert.IsDBNull(dt.Rows[0]["IUSER"]) ? null : (string)dt.Rows[0]["IUSER"];
                    retObj._udate = Convert.IsDBNull(dt.Rows[0]["UDATE"]) ? (DateTime?)null : (DateTime?)dt.Rows[0]["UDATE"];
                    retObj._uuser = Convert.IsDBNull(dt.Rows[0]["UUSER"]) ? null : (string)dt.Rows[0]["UUSER"];
                    retObj._empid = Convert.IsDBNull(dt.Rows[0]["EMPID"]) ? null : (string)dt.Rows[0]["EMPID"];
                    retObj._mailcode = Convert.IsDBNull(dt.Rows[0]["MAILCODE"]) ? null : (string)dt.Rows[0]["MAILCODE"];
                    retObj._position = Convert.IsDBNull(dt.Rows[0]["POSITION"]) ? (decimal?)null : (decimal?)dt.Rows[0]["POSITION"];
                    retObj._ctynb = Convert.IsDBNull(dt.Rows[0]["CTY_NB"]) ? (Int64?)null : (Int64?)dt.Rows[0]["CTY_NB"];
                }
                return retObj;
            }
            catch (Exception ex)
            {
                throw new Exception("DAOAppmgrUsers::Error Occurred:SelectOneByUsername:" + ex.Message, ex);
            }
            finally
            {
                staticConnection.Close();
                command.Dispose();
            }
        }

        ///<Summary>
        ///Delete one row by unique constraint
        ///This method deletes one row from the table USERS based on a unique constraint
        ///</Summary>
        ///<returns>
        ///void
        ///</returns>
        ///<parameters>
        ///string username
        ///</parameters>
        public virtual void DeleteOneByUsername(string username)
        {
            OracleCommand command = new OracleCommand();
            command.CommandText = "APPMGR.PUSERS.CTPR_DELETEONEBYUSERNAME";
            command.CommandType = CommandType.StoredProcedure;
            command.Connection = _connectionProvider.Connection;

            try
            {
                command.Parameters.Add(new OracleParameter("P_USERNAME", OracleDbType.Varchar2, 40, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Proposed, (object)_username ?? (object)DBNull.Value));

                command.ExecuteNonQuery();


            }
            catch (Exception ex)
            {
                throw new Exception("DAOAppmgrUsers::Error Occurred:DeleteOneByUsername:" + ex.Message, ex);
            }
            finally
            {
                command.Dispose();
            }
        }

        ///<Summary>
        ///Select all rows by foreign key
        ///This method returns all data rows in the table USERS based on a foreign key
        ///</Summary>
        ///<returns>
        ///List-DAOAppmgrUsers.
        ///</returns>
        ///<parameters>
        ///Int64? deptnb
        ///</parameters>
        public static List<DAOUsers> SelectAllByDeptnb(Int64? deptnb)
        {
            OracleCommand command = new OracleCommand();
            command.CommandText = "APPMGR.PUSERS.CTPR_GETBYDEPTNB";
            command.CommandType = CommandType.StoredProcedure;
            OracleConnection staticConnection = StaticSqlConnection;
            command.Connection = staticConnection;

            DataTable dt = new DataTable("USERS");
            OracleDataAdapter sqlAdapter = new OracleDataAdapter(command);
            try
            {
                command.Parameters.Add(new OracleParameter("P_DEPTNB", OracleDbType.Int64, 22, ParameterDirection.Input, true, 10, 0, "", DataRowVersion.Proposed, (object)deptnb ?? (object)DBNull.Value));

                OracleParameter refCursor = new OracleParameter("RESULTS", OracleDbType.RefCursor);
                refCursor.Direction = ParameterDirection.Output;
                command.Parameters.Add(refCursor);

                staticConnection.Open();
                sqlAdapter.Fill(dt);


                List<DAOUsers> objList = new List<DAOUsers>();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        DAOUsers retObj = new DAOUsers();
                        retObj._nb = Convert.IsDBNull(row["NB"]) ? (Int64?)null : (Int64?)row["NB"];
                        retObj._username = Convert.IsDBNull(row["USERNAME"]) ? null : (string)row["USERNAME"];
                        retObj._afirstname = Convert.IsDBNull(row["AFIRSTNAME"]) ? null : (string)row["AFIRSTNAME"];
                        retObj._alastname = Convert.IsDBNull(row["ALASTNAME"]) ? null : (string)row["ALASTNAME"];
                        retObj._password = Convert.IsDBNull(row["PASSWORD"]) ? null : (string)row["PASSWORD"];
                        retObj._created = Convert.IsDBNull(row["CREATED"]) ? (DateTime?)null : (DateTime?)row["CREATED"];
                        retObj._gendr = Convert.IsDBNull(row["GENDR"]) ? null : (string)row["GENDR"];
                        retObj._locked = Convert.IsDBNull(row["LOCKED"]) ? null : (string)row["LOCKED"];
                        retObj._deptnb = Convert.IsDBNull(row["DEPTNB"]) ? (Int64?)null : (Int64?)row["DEPTNB"];
                        retObj._prefix = Convert.IsDBNull(row["PREFIX"]) ? null : (string)row["PREFIX"];
                        retObj._visible = Convert.IsDBNull(row["VISIBLE"]) ? (Int64?)null : (Int64?)row["VISIBLE"];
                        retObj._ssystem = Convert.IsDBNull(row["SSYSTEM"]) ? (Int64?)null : (Int64?)row["SSYSTEM"];
                        retObj._securlevel = Convert.IsDBNull(row["SECURLEVEL"]) ? (Int16?)null : (Int16?)row["SECURLEVEL"];
                        retObj._canchpasswd = Convert.IsDBNull(row["CANCHPASSWD"]) ? (Int64?)null : (Int64?)row["CANCHPASSWD"];
                        retObj._mustchpasswd = Convert.IsDBNull(row["MUSTCHPASSWD"]) ? (Int64?)null : (Int64?)row["MUSTCHPASSWD"];
                        retObj._sessionco = Convert.IsDBNull(row["SESSIONCO"]) ? (Int64?)null : (Int64?)row["SESSIONCO"];
                        retObj._carsesco = Convert.IsDBNull(row["CARSESCO"]) ? (Int64?)null : (Int64?)row["CARSESCO"];
                        retObj._idltim = Convert.IsDBNull(row["IDLTIM"]) ? (Int64?)null : (Int64?)row["IDLTIM"];
                        retObj._passwordBond = Convert.IsDBNull(row["PASSWORD_BOND"]) ? null : (string)row["PASSWORD_BOND"];
                        retObj._mechanicPass = Convert.IsDBNull(row["MECHANIC_PASS"]) ? null : (string)row["MECHANIC_PASS"];
                        retObj._insurancePass = Convert.IsDBNull(row["INSURANCE_PASS"]) ? null : (string)row["INSURANCE_PASS"];
                        retObj._idate = Convert.IsDBNull(row["IDATE"]) ? (DateTime?)null : (DateTime?)row["IDATE"];
                        retObj._iuser = Convert.IsDBNull(row["IUSER"]) ? null : (string)row["IUSER"];
                        retObj._udate = Convert.IsDBNull(row["UDATE"]) ? (DateTime?)null : (DateTime?)row["UDATE"];
                        retObj._uuser = Convert.IsDBNull(row["UUSER"]) ? null : (string)row["UUSER"];
                        retObj._empid = Convert.IsDBNull(row["EMPID"]) ? null : (string)row["EMPID"];
                        retObj._mailcode = Convert.IsDBNull(row["MAILCODE"]) ? null : (string)row["MAILCODE"];
                        retObj._position = Convert.IsDBNull(row["POSITION"]) ? (decimal?)null : (decimal?)row["POSITION"];
                        retObj._ctynb = Convert.IsDBNull(row["CTY_NB"]) ? (Int64?)null : (Int64?)row["CTY_NB"];
                        objList.Add(retObj);
                    }
                }
                return objList;
            }
            catch (Exception ex)
            {
                throw new Exception("DAOAppmgrUsers::Error Occurred:SelectAllByDeptnb:" + ex.Message, ex);
            }
            finally
            {
                staticConnection.Close();
                command.Dispose();
            }
        }

        ///<Summary>
        ///</Summary>
        ///<returns>
        ///Int32
        ///</returns>
        ///<parameters>
        ///Int64? deptnb
        ///</parameters>
        public static Int32 SelectAllByDeptnbCount(Int64? deptnb)
        {
            OracleCommand command = new OracleCommand();
            command.CommandText = "APPMGR.PUSERS.CTPR_GETBYDEPTNBCOUNT";
            command.CommandType = CommandType.StoredProcedure;
            OracleConnection staticConnection = StaticSqlConnection;
            command.Connection = staticConnection;

            try
            {
                command.Parameters.Add(new OracleParameter("P_DEPTNB", OracleDbType.Int64, 22, ParameterDirection.Input, true, 10, 0, "", DataRowVersion.Proposed, (object)deptnb ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_COUNT", OracleDbType.Int32, 4, ParameterDirection.Output, false, 10, 0, "", DataRowVersion.Proposed, null));

                staticConnection.Open();
                command.ExecuteNonQuery();

                Int32 retCount = (Int32)(OracleDecimal)command.Parameters["P_COUNT"].Value;

                return retCount;
            }
            catch (Exception ex)
            {
                throw new Exception("DAOAppmgrUsers::Error Occurred:SelectAllByDeptnbCount:" + ex.Message, ex);
            }
            finally
            {
                staticConnection.Close();
                command.Dispose();
            }
        }

        ///<Summary>
        ///This method deletes all rows in the table USERS with a given foreign key
        ///This method deletes all rows in the table USERS with a given foreign key
        ///</Summary>
        ///<returns>
        ///void
        ///</returns>
        ///<parameters>
        ///TxConnectionProvider connectionProvider, Int64? deptnb
        ///</parameters>
        public static void DeleteAllByDeptnb(OraSync_TxConnectionProvider connectionProvider, Int64? deptnb)
        {
            OracleCommand command = new OracleCommand();
            command.CommandText = "APPMGR.PUSERS.CTPR_DELETEBYDEPTNB";
            command.CommandType = CommandType.StoredProcedure;
            command.Connection = connectionProvider.Connection;
            command.Transaction = connectionProvider.CurrentTransaction;

            try
            {
                command.Parameters.Add(new OracleParameter("P_DEPTNB", OracleDbType.Int64, 22, ParameterDirection.Input, true, 10, 0, "", DataRowVersion.Proposed, (object)deptnb ?? (object)DBNull.Value));

                command.ExecuteNonQuery();


            }
            catch (Exception ex)
            {
                throw new Exception("DAOAppmgrUsers::Error Occurred:DeleteAllByDeptnb:" + ex.Message, ex);
            }
            finally
            {
                command.Dispose();
            }
        }

        ///<Summary>
        ///Insert a new row
        ///This method saves a new object to the table USERS
        ///</Summary>
        ///<returns>
        ///void
        ///</returns>
        ///<parameters>
        ///
        ///</parameters>
        public virtual void Insert()
        {
            OracleCommand command = new OracleCommand();
            command.CommandText = "APPMGR.PUSERS.CTPR_INSERTONE";
            command.CommandType = CommandType.StoredProcedure;
            command.Connection = _connectionProvider.Connection;

            try
            {
                command.Parameters.Add(new OracleParameter("P_NB", OracleDbType.Int64, 22, ParameterDirection.InputOutput, true, 10, 0, "", DataRowVersion.Proposed, _nb));
                command.Parameters.Add(new OracleParameter("P_USERNAME", OracleDbType.Varchar2, 40, ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Proposed, _username));
                command.Parameters.Add(new OracleParameter("P_AFIRSTNAME", OracleDbType.Varchar2, 40, ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Proposed, _afirstname));
                command.Parameters.Add(new OracleParameter("P_ALASTNAME", OracleDbType.Varchar2, 40, ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Proposed, _alastname));
                command.Parameters.Add(new OracleParameter("P_PASSWORD", OracleDbType.Varchar2, 200, ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Proposed, _password));
                command.Parameters.Add(new OracleParameter("P_CREATED", OracleDbType.Date, 7, ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Proposed, _created));
                command.Parameters.Add(new OracleParameter("P_GENDR", OracleDbType.Varchar2, 5, ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Proposed, _gendr));
                command.Parameters.Add(new OracleParameter("P_LOCKED", OracleDbType.Varchar2, 3, ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Proposed, _locked));
                command.Parameters.Add(new OracleParameter("P_DEPTNB", OracleDbType.Int64, 22, ParameterDirection.InputOutput, true, 10, 0, "", DataRowVersion.Proposed, _deptnb));
                command.Parameters.Add(new OracleParameter("P_PREFIX", OracleDbType.Varchar2, 20, ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Proposed, _prefix));
                command.Parameters.Add(new OracleParameter("P_VISIBLE", OracleDbType.Int64, 22, ParameterDirection.InputOutput, true, 10, 0, "", DataRowVersion.Proposed, _visible));
                command.Parameters.Add(new OracleParameter("P_SSYSTEM", OracleDbType.Int64, 22, ParameterDirection.InputOutput, true, 10, 0, "", DataRowVersion.Proposed, _ssystem));
                command.Parameters.Add(new OracleParameter("P_SECURLEVEL", OracleDbType.Int16, 22, ParameterDirection.InputOutput, true, 2, 0, "", DataRowVersion.Proposed, _securlevel));
                command.Parameters.Add(new OracleParameter("P_CANCHPASSWD", OracleDbType.Int64, 22, ParameterDirection.InputOutput, true, 10, 0, "", DataRowVersion.Proposed, _canchpasswd));
                command.Parameters.Add(new OracleParameter("P_MUSTCHPASSWD", OracleDbType.Int64, 22, ParameterDirection.InputOutput, true, 10, 0, "", DataRowVersion.Proposed, _mustchpasswd));
                command.Parameters.Add(new OracleParameter("P_SESSIONCO", OracleDbType.Int64, 22, ParameterDirection.InputOutput, true, 10, 0, "", DataRowVersion.Proposed, _sessionco));
                command.Parameters.Add(new OracleParameter("P_CARSESCO", OracleDbType.Int64, 22, ParameterDirection.InputOutput, true, 10, 0, "", DataRowVersion.Proposed, _carsesco));
                command.Parameters.Add(new OracleParameter("P_IDLTIM", OracleDbType.Int64, 22, ParameterDirection.InputOutput, true, 10, 0, "", DataRowVersion.Proposed, _idltim));
                command.Parameters.Add(new OracleParameter("P_PASSWORD_BOND", OracleDbType.Varchar2, 100, ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Proposed, _passwordBond));
                command.Parameters.Add(new OracleParameter("P_MECHANIC_PASS", OracleDbType.Varchar2, 200, ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Proposed, _mechanicPass));
                command.Parameters.Add(new OracleParameter("P_INSURANCE_PASS", OracleDbType.Varchar2, 200, ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Proposed, _insurancePass));
                command.Parameters.Add(new OracleParameter("P_IDATE", OracleDbType.Date, 7, ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Proposed, _idate));
                command.Parameters.Add(new OracleParameter("P_IUSER", OracleDbType.Varchar2, 200, ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Proposed, _iuser));
                command.Parameters.Add(new OracleParameter("P_UDATE", OracleDbType.Date, 7, ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Proposed, _udate));
                command.Parameters.Add(new OracleParameter("P_UUSER", OracleDbType.Varchar2, 200, ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Proposed, _uuser));
                command.Parameters.Add(new OracleParameter("P_EMPID", OracleDbType.Varchar2, 10, ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Proposed, _empid));
                command.Parameters.Add(new OracleParameter("P_MAILCODE", OracleDbType.Varchar2, 10, ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Proposed, _mailcode));
                command.Parameters.Add(new OracleParameter("P_POSITION", OracleDbType.Decimal, 22, ParameterDirection.InputOutput, true, 28, 4, "", DataRowVersion.Proposed, _position));
                command.Parameters.Add(new OracleParameter("P_CTY_NB", OracleDbType.Int64, 22, ParameterDirection.InputOutput, true, 10, 0, "", DataRowVersion.Proposed, _ctynb));

                command.ExecuteNonQuery();

                _nb = (Int64?)(decimal?)UnboxOracleParam(command.Parameters["P_NB"].Value);
                _username = (string)UnboxOracleParam(command.Parameters["P_USERNAME"].Value);
                _afirstname = (string)UnboxOracleParam(command.Parameters["P_AFIRSTNAME"].Value);
                _alastname = (string)UnboxOracleParam(command.Parameters["P_ALASTNAME"].Value);
                _password = (string)UnboxOracleParam(command.Parameters["P_PASSWORD"].Value);
                _created = (DateTime?)UnboxOracleParam(command.Parameters["P_CREATED"].Value);
                _gendr = (string)UnboxOracleParam(command.Parameters["P_GENDR"].Value);
                _locked = (string)UnboxOracleParam(command.Parameters["P_LOCKED"].Value);
                _deptnb = (Int64?)(decimal?)UnboxOracleParam(command.Parameters["P_DEPTNB"].Value);
                _prefix = (string)UnboxOracleParam(command.Parameters["P_PREFIX"].Value);
                _visible = (Int64?)(decimal?)UnboxOracleParam(command.Parameters["P_VISIBLE"].Value);
                _ssystem = (Int64?)(decimal?)UnboxOracleParam(command.Parameters["P_SSYSTEM"].Value);
                _securlevel = (Int16?)(decimal?)UnboxOracleParam(command.Parameters["P_SECURLEVEL"].Value);
                _canchpasswd = (Int64?)(decimal?)UnboxOracleParam(command.Parameters["P_CANCHPASSWD"].Value);
                _mustchpasswd = (Int64?)(decimal?)UnboxOracleParam(command.Parameters["P_MUSTCHPASSWD"].Value);
                _sessionco = (Int64?)(decimal?)UnboxOracleParam(command.Parameters["P_SESSIONCO"].Value);
                _carsesco = (Int64?)(decimal?)UnboxOracleParam(command.Parameters["P_CARSESCO"].Value);
                _idltim = (Int64?)(decimal?)UnboxOracleParam(command.Parameters["P_IDLTIM"].Value);
                _passwordBond = (string)UnboxOracleParam(command.Parameters["P_PASSWORD_BOND"].Value);
                _mechanicPass = (string)UnboxOracleParam(command.Parameters["P_MECHANIC_PASS"].Value);
                _insurancePass = (string)UnboxOracleParam(command.Parameters["P_INSURANCE_PASS"].Value);
                _idate = (DateTime?)UnboxOracleParam(command.Parameters["P_IDATE"].Value);
                _iuser = (string)UnboxOracleParam(command.Parameters["P_IUSER"].Value);
                _udate = (DateTime?)UnboxOracleParam(command.Parameters["P_UDATE"].Value);
                _uuser = (string)UnboxOracleParam(command.Parameters["P_UUSER"].Value);
                _empid = (string)UnboxOracleParam(command.Parameters["P_EMPID"].Value);
                _mailcode = (string)UnboxOracleParam(command.Parameters["P_MAILCODE"].Value);
                _position = (decimal?)UnboxOracleParam(command.Parameters["P_POSITION"].Value);
                _ctynb = (Int64?)(decimal?)UnboxOracleParam(command.Parameters["P_CTY_NB"].Value);

            }
            catch (Exception ex)
            {
                throw new Exception("DAOAppmgrUsers::Error Occurred:Insert:" + ex.Message, ex);
            }
            finally
            {
                command.Dispose();
            }
        }

        ///<Summary>
        ///Select all rows
        ///This method returns all data rows in the table USERS
        ///</Summary>
        ///<returns>
        ///List-DAOAppmgrUsers.
        ///</returns>
        ///<parameters>
        ///
        ///</parameters>
        public static List<DAOUsers> SelectAll()
        {
            OracleCommand command = new OracleCommand();
            command.CommandText = "APPMGR.PUSERS.CTPR_GETALL";
            command.CommandType = CommandType.StoredProcedure;
            OracleConnection staticConnection = StaticSqlConnection;
            command.Connection = staticConnection;

            DataTable dt = new DataTable("USERS");
            OracleDataAdapter sqlAdapter = new OracleDataAdapter(command);
            try
            {

                OracleParameter refCursor = new OracleParameter("RESULTS", OracleDbType.RefCursor);
                refCursor.Direction = ParameterDirection.Output;
                command.Parameters.Add(refCursor);

                staticConnection.Open();
                sqlAdapter.Fill(dt);


                List<DAOUsers> objList = new List<DAOUsers>();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        DAOUsers retObj = new DAOUsers();
                        retObj._nb = Convert.IsDBNull(row["NB"]) ? (Int64?)null : (Int64?)row["NB"];
                        retObj._username = Convert.IsDBNull(row["USERNAME"]) ? null : (string)row["USERNAME"];
                        retObj._afirstname = Convert.IsDBNull(row["AFIRSTNAME"]) ? null : (string)row["AFIRSTNAME"];
                        retObj._alastname = Convert.IsDBNull(row["ALASTNAME"]) ? null : (string)row["ALASTNAME"];
                        retObj._password = Convert.IsDBNull(row["PASSWORD"]) ? null : (string)row["PASSWORD"];
                        retObj._created = Convert.IsDBNull(row["CREATED"]) ? (DateTime?)null : (DateTime?)row["CREATED"];
                        retObj._gendr = Convert.IsDBNull(row["GENDR"]) ? null : (string)row["GENDR"];
                        retObj._locked = Convert.IsDBNull(row["LOCKED"]) ? null : (string)row["LOCKED"];
                        retObj._deptnb = Convert.IsDBNull(row["DEPTNB"]) ? (Int64?)null : (Int64?)row["DEPTNB"];
                        retObj._prefix = Convert.IsDBNull(row["PREFIX"]) ? null : (string)row["PREFIX"];
                        retObj._visible = Convert.IsDBNull(row["VISIBLE"]) ? (Int64?)null : (Int64?)row["VISIBLE"];
                        retObj._ssystem = Convert.IsDBNull(row["SSYSTEM"]) ? (Int64?)null : (Int64?)row["SSYSTEM"];
                        retObj._securlevel = Convert.IsDBNull(row["SECURLEVEL"]) ? (Int16?)null : (Int16?)row["SECURLEVEL"];
                        retObj._canchpasswd = Convert.IsDBNull(row["CANCHPASSWD"]) ? (Int64?)null : (Int64?)row["CANCHPASSWD"];
                        retObj._mustchpasswd = Convert.IsDBNull(row["MUSTCHPASSWD"]) ? (Int64?)null : (Int64?)row["MUSTCHPASSWD"];
                        retObj._sessionco = Convert.IsDBNull(row["SESSIONCO"]) ? (Int64?)null : (Int64?)row["SESSIONCO"];
                        retObj._carsesco = Convert.IsDBNull(row["CARSESCO"]) ? (Int64?)null : (Int64?)row["CARSESCO"];
                        retObj._idltim = Convert.IsDBNull(row["IDLTIM"]) ? (Int64?)null : (Int64?)row["IDLTIM"];
                        retObj._passwordBond = Convert.IsDBNull(row["PASSWORD_BOND"]) ? null : (string)row["PASSWORD_BOND"];
                        retObj._mechanicPass = Convert.IsDBNull(row["MECHANIC_PASS"]) ? null : (string)row["MECHANIC_PASS"];
                        retObj._insurancePass = Convert.IsDBNull(row["INSURANCE_PASS"]) ? null : (string)row["INSURANCE_PASS"];
                        retObj._idate = Convert.IsDBNull(row["IDATE"]) ? (DateTime?)null : (DateTime?)row["IDATE"];
                        retObj._iuser = Convert.IsDBNull(row["IUSER"]) ? null : (string)row["IUSER"];
                        retObj._udate = Convert.IsDBNull(row["UDATE"]) ? (DateTime?)null : (DateTime?)row["UDATE"];
                        retObj._uuser = Convert.IsDBNull(row["UUSER"]) ? null : (string)row["UUSER"];
                        retObj._empid = Convert.IsDBNull(row["EMPID"]) ? null : (string)row["EMPID"];
                        retObj._mailcode = Convert.IsDBNull(row["MAILCODE"]) ? null : (string)row["MAILCODE"];
                        retObj._position = Convert.IsDBNull(row["POSITION"]) ? (decimal?)null : (decimal?)row["POSITION"];
                        retObj._ctynb = Convert.IsDBNull(row["CTY_NB"]) ? (Int64?)null : (Int64?)row["CTY_NB"];
                        objList.Add(retObj);
                    }
                }
                return objList;
            }
            catch (Exception ex)
            {
                throw new Exception("DAOAppmgrUsers::Error Occurred:SelectAll:" + ex.Message, ex);
            }
            finally
            {
                staticConnection.Close();
                command.Dispose();
            }
        }

        ///<Summary>
        ///</Summary>
        ///<returns>
        ///Int32
        ///</returns>
        ///<parameters>
        ///
        ///</parameters>
        public static Int32 SelectAllCount()
        {
            OracleCommand command = new OracleCommand();
            command.CommandText = "APPMGR.PUSERS.CTPR_GETALLCOUNT";
            command.CommandType = CommandType.StoredProcedure;
            OracleConnection staticConnection = StaticSqlConnection;
            command.Connection = staticConnection;

            try
            {
                command.Parameters.Add(new OracleParameter("P_COUNT", OracleDbType.Int32, 4, ParameterDirection.Output, false, 10, 0, "", DataRowVersion.Proposed, null));

                staticConnection.Open();
                command.ExecuteNonQuery();

                Int32 retCount = (Int32)(OracleDecimal)command.Parameters["P_COUNT"].Value;

                return retCount;
            }
            catch (Exception ex)
            {
                throw new Exception("DAOAppmgrUsers::Error Occurred:SelectAllCount:" + ex.Message, ex);
            }
            finally
            {
                staticConnection.Close();
                command.Dispose();
            }
        }

        ///<Summary>
        ///</Summary>
        ///<returns>
        ///List-DAOAppmgrUsers.
        ///</returns>
        ///<parameters>
        ///DAOAppmgrUsers daoAppmgrUsers
        ///</parameters>
        public static List<DAOUsers> SelectAllBySearchFields(DAOUsers daoAppmgrUsers)
        {
            OracleCommand command = new OracleCommand();
            command.CommandText = "APPMGR.PUSERS.CTPR_GETBYSEARCHFIELDS";
            command.CommandType = CommandType.StoredProcedure;
            OracleConnection staticConnection = StaticSqlConnection;
            command.Connection = staticConnection;

            DataTable dt = new DataTable("USERS");
            OracleDataAdapter sqlAdapter = new OracleDataAdapter(command);
            try
            {
                command.Parameters.Add(new OracleParameter("P_NB", OracleDbType.Int64, 22, ParameterDirection.Input, false, 10, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Nb ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_USERNAME", OracleDbType.Varchar2, 40, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Username ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_AFIRSTNAME", OracleDbType.Varchar2, 40, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Afirstname ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_ALASTNAME", OracleDbType.Varchar2, 40, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Alastname ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_PASSWORD", OracleDbType.Varchar2, 200, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Password ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_CREATED", OracleDbType.Date, 7, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Created ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_GENDR", OracleDbType.Varchar2, 5, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Gendr ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_LOCKED", OracleDbType.Varchar2, 3, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Locked ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_DEPTNB", OracleDbType.Int64, 22, ParameterDirection.Input, true, 10, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Deptnb ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_PREFIX", OracleDbType.Varchar2, 20, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Prefix ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_VISIBLE", OracleDbType.Int64, 22, ParameterDirection.Input, true, 10, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Visible ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_SSYSTEM", OracleDbType.Int64, 22, ParameterDirection.Input, true, 10, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Ssystem ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_SECURLEVEL", OracleDbType.Int16, 22, ParameterDirection.Input, true, 2, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Securlevel ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_CANCHPASSWD", OracleDbType.Int64, 22, ParameterDirection.Input, true, 10, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Canchpasswd ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_MUSTCHPASSWD", OracleDbType.Int64, 22, ParameterDirection.Input, true, 10, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Mustchpasswd ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_SESSIONCO", OracleDbType.Int64, 22, ParameterDirection.Input, true, 10, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Sessionco ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_CARSESCO", OracleDbType.Int64, 22, ParameterDirection.Input, true, 10, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Carsesco ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_IDLTIM", OracleDbType.Int64, 22, ParameterDirection.Input, true, 10, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Idltim ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_PASSWORD_BOND", OracleDbType.Varchar2, 100, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.PasswordBond ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_MECHANIC_PASS", OracleDbType.Varchar2, 200, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.MechanicPass ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_INSURANCE_PASS", OracleDbType.Varchar2, 200, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.InsurancePass ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_IDATE", OracleDbType.Date, 7, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Idate ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_IUSER", OracleDbType.Varchar2, 200, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Iuser ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_UDATE", OracleDbType.Date, 7, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Udate ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_UUSER", OracleDbType.Varchar2, 200, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Uuser ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_EMPID", OracleDbType.Varchar2, 10, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Empid ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_MAILCODE", OracleDbType.Varchar2, 10, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Mailcode ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_POSITION", OracleDbType.Decimal, 22, ParameterDirection.Input, true, 28, 4, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Position ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_CTY_NB", OracleDbType.Int64, 22, ParameterDirection.Input, true, 10, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Ctynb ?? (object)DBNull.Value));

                OracleParameter refCursor = new OracleParameter("RESULTS", OracleDbType.RefCursor);
                refCursor.Direction = ParameterDirection.Output;
                command.Parameters.Add(refCursor);

                staticConnection.Open();
                sqlAdapter.Fill(dt);


                List<DAOUsers> objList = new List<DAOUsers>();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        DAOUsers retObj = new DAOUsers();
                        retObj._nb = Convert.IsDBNull(row["NB"]) ? (Int64?)null : (Int64?)row["NB"];
                        retObj._username = Convert.IsDBNull(row["USERNAME"]) ? null : (string)row["USERNAME"];
                        retObj._afirstname = Convert.IsDBNull(row["AFIRSTNAME"]) ? null : (string)row["AFIRSTNAME"];
                        retObj._alastname = Convert.IsDBNull(row["ALASTNAME"]) ? null : (string)row["ALASTNAME"];
                        retObj._password = Convert.IsDBNull(row["PASSWORD"]) ? null : (string)row["PASSWORD"];
                        retObj._created = Convert.IsDBNull(row["CREATED"]) ? (DateTime?)null : (DateTime?)row["CREATED"];
                        retObj._gendr = Convert.IsDBNull(row["GENDR"]) ? null : (string)row["GENDR"];
                        retObj._locked = Convert.IsDBNull(row["LOCKED"]) ? null : (string)row["LOCKED"];
                        retObj._deptnb = Convert.IsDBNull(row["DEPTNB"]) ? (Int64?)null : (Int64?)row["DEPTNB"];
                        retObj._prefix = Convert.IsDBNull(row["PREFIX"]) ? null : (string)row["PREFIX"];
                        retObj._visible = Convert.IsDBNull(row["VISIBLE"]) ? (Int64?)null : (Int64?)row["VISIBLE"];
                        retObj._ssystem = Convert.IsDBNull(row["SSYSTEM"]) ? (Int64?)null : (Int64?)row["SSYSTEM"];
                        retObj._securlevel = Convert.IsDBNull(row["SECURLEVEL"]) ? (Int16?)null : (Int16?)row["SECURLEVEL"];
                        retObj._canchpasswd = Convert.IsDBNull(row["CANCHPASSWD"]) ? (Int64?)null : (Int64?)row["CANCHPASSWD"];
                        retObj._mustchpasswd = Convert.IsDBNull(row["MUSTCHPASSWD"]) ? (Int64?)null : (Int64?)row["MUSTCHPASSWD"];
                        retObj._sessionco = Convert.IsDBNull(row["SESSIONCO"]) ? (Int64?)null : (Int64?)row["SESSIONCO"];
                        retObj._carsesco = Convert.IsDBNull(row["CARSESCO"]) ? (Int64?)null : (Int64?)row["CARSESCO"];
                        retObj._idltim = Convert.IsDBNull(row["IDLTIM"]) ? (Int64?)null : (Int64?)row["IDLTIM"];
                        retObj._passwordBond = Convert.IsDBNull(row["PASSWORD_BOND"]) ? null : (string)row["PASSWORD_BOND"];
                        retObj._mechanicPass = Convert.IsDBNull(row["MECHANIC_PASS"]) ? null : (string)row["MECHANIC_PASS"];
                        retObj._insurancePass = Convert.IsDBNull(row["INSURANCE_PASS"]) ? null : (string)row["INSURANCE_PASS"];
                        retObj._idate = Convert.IsDBNull(row["IDATE"]) ? (DateTime?)null : (DateTime?)row["IDATE"];
                        retObj._iuser = Convert.IsDBNull(row["IUSER"]) ? null : (string)row["IUSER"];
                        retObj._udate = Convert.IsDBNull(row["UDATE"]) ? (DateTime?)null : (DateTime?)row["UDATE"];
                        retObj._uuser = Convert.IsDBNull(row["UUSER"]) ? null : (string)row["UUSER"];
                        retObj._empid = Convert.IsDBNull(row["EMPID"]) ? null : (string)row["EMPID"];
                        retObj._mailcode = Convert.IsDBNull(row["MAILCODE"]) ? null : (string)row["MAILCODE"];
                        retObj._position = Convert.IsDBNull(row["POSITION"]) ? (decimal?)null : (decimal?)row["POSITION"];
                        retObj._ctynb = Convert.IsDBNull(row["CTY_NB"]) ? (Int64?)null : (Int64?)row["CTY_NB"];
                        objList.Add(retObj);
                    }
                }
                return objList;
            }
            catch (Exception ex)
            {
                throw new Exception("DAOAppmgrUsers::Error Occurred:SelectAllBySearchFields:" + ex.Message, ex);
            }
            finally
            {
                staticConnection.Close();
                command.Dispose();
            }
        }

        ///<Summary>
        ///</Summary>
        ///<returns>
        ///Int32
        ///</returns>
        ///<parameters>
        ///DAOAppmgrUsers daoAppmgrUsers
        ///</parameters>
        public static Int32 SelectAllBySearchFieldsCount(DAOUsers daoAppmgrUsers)
        {
            OracleCommand command = new OracleCommand();
            command.CommandText = "APPMGR.PUSERS.CTPR_GETBYSEARCHFIELDSCOUNT";
            command.CommandType = CommandType.StoredProcedure;
            OracleConnection staticConnection = StaticSqlConnection;
            command.Connection = staticConnection;

            try
            {
                command.Parameters.Add(new OracleParameter("P_NB", OracleDbType.Int64, 22, ParameterDirection.Input, false, 10, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Nb ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_USERNAME", OracleDbType.Varchar2, 40, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Username ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_AFIRSTNAME", OracleDbType.Varchar2, 40, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Afirstname ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_ALASTNAME", OracleDbType.Varchar2, 40, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Alastname ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_PASSWORD", OracleDbType.Varchar2, 200, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Password ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_CREATED", OracleDbType.Date, 7, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Created ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_GENDR", OracleDbType.Varchar2, 5, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Gendr ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_LOCKED", OracleDbType.Varchar2, 3, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Locked ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_DEPTNB", OracleDbType.Int64, 22, ParameterDirection.Input, true, 10, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Deptnb ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_PREFIX", OracleDbType.Varchar2, 20, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Prefix ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_VISIBLE", OracleDbType.Int64, 22, ParameterDirection.Input, true, 10, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Visible ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_SSYSTEM", OracleDbType.Int64, 22, ParameterDirection.Input, true, 10, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Ssystem ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_SECURLEVEL", OracleDbType.Int16, 22, ParameterDirection.Input, true, 2, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Securlevel ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_CANCHPASSWD", OracleDbType.Int64, 22, ParameterDirection.Input, true, 10, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Canchpasswd ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_MUSTCHPASSWD", OracleDbType.Int64, 22, ParameterDirection.Input, true, 10, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Mustchpasswd ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_SESSIONCO", OracleDbType.Int64, 22, ParameterDirection.Input, true, 10, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Sessionco ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_CARSESCO", OracleDbType.Int64, 22, ParameterDirection.Input, true, 10, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Carsesco ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_IDLTIM", OracleDbType.Int64, 22, ParameterDirection.Input, true, 10, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Idltim ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_PASSWORD_BOND", OracleDbType.Varchar2, 100, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.PasswordBond ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_MECHANIC_PASS", OracleDbType.Varchar2, 200, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.MechanicPass ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_INSURANCE_PASS", OracleDbType.Varchar2, 200, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.InsurancePass ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_IDATE", OracleDbType.Date, 7, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Idate ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_IUSER", OracleDbType.Varchar2, 200, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Iuser ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_UDATE", OracleDbType.Date, 7, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Udate ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_UUSER", OracleDbType.Varchar2, 200, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Uuser ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_EMPID", OracleDbType.Varchar2, 10, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Empid ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_MAILCODE", OracleDbType.Varchar2, 10, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Mailcode ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_POSITION", OracleDbType.Decimal, 22, ParameterDirection.Input, true, 28, 4, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Position ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("P_COUNT", OracleDbType.Int32, 4, ParameterDirection.Output, false, 10, 0, "", DataRowVersion.Proposed, null));
                command.Parameters.Add(new OracleParameter("P_CTY_NB", OracleDbType.Int64, 22, ParameterDirection.Input, true, 10, 0, "", DataRowVersion.Proposed, (object)daoAppmgrUsers.Ctynb ?? (object)DBNull.Value));

                staticConnection.Open();
                command.ExecuteNonQuery();

                Int32 retCount = (Int32)(OracleDecimal)command.Parameters["P_COUNT"].Value;

                return retCount;
            }
            catch (Exception ex)
            {
                throw new Exception("DAOAppmgrUsers::Error Occurred:SelectAllBySearchFieldsCount:" + ex.Message, ex);
            }
            finally
            {
                staticConnection.Close();
                command.Dispose();
            }
        }

        ///<Summary>
        ///Update one row by primary key(s)
        ///This method allows the object to update itself in the table USERS based on its primary key(s)
        ///</Summary>
        ///<returns>
        ///void
        ///</returns>
        ///<parameters>
        ///
        ///</parameters>
        public virtual void Update()
        {
            OracleCommand command = new OracleCommand();
            command.CommandText = "APPMGR.PUSERS.CTPR_UPDATEONE";
            command.CommandType = CommandType.StoredProcedure;
            command.Connection = _connectionProvider.Connection;

            try
            {
                command.Parameters.Add(new OracleParameter("P_NB", OracleDbType.Int64, 22, ParameterDirection.InputOutput, true, 10, 0, "", DataRowVersion.Proposed, _nb));
                command.Parameters.Add(new OracleParameter("P_USERNAME", OracleDbType.Varchar2, 40, ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Proposed, _username));
                command.Parameters.Add(new OracleParameter("P_AFIRSTNAME", OracleDbType.Varchar2, 40, ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Proposed, _afirstname));
                command.Parameters.Add(new OracleParameter("P_ALASTNAME", OracleDbType.Varchar2, 40, ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Proposed, _alastname));
                command.Parameters.Add(new OracleParameter("P_PASSWORD", OracleDbType.Varchar2, 200, ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Proposed, _password));
                command.Parameters.Add(new OracleParameter("P_CREATED", OracleDbType.Date, 7, ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Proposed, _created));
                command.Parameters.Add(new OracleParameter("P_GENDR", OracleDbType.Varchar2, 5, ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Proposed, _gendr));
                command.Parameters.Add(new OracleParameter("P_LOCKED", OracleDbType.Varchar2, 3, ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Proposed, _locked));
                command.Parameters.Add(new OracleParameter("P_DEPTNB", OracleDbType.Int64, 22, ParameterDirection.InputOutput, true, 10, 0, "", DataRowVersion.Proposed, _deptnb));
                command.Parameters.Add(new OracleParameter("P_PREFIX", OracleDbType.Varchar2, 20, ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Proposed, _prefix));
                command.Parameters.Add(new OracleParameter("P_VISIBLE", OracleDbType.Int64, 22, ParameterDirection.InputOutput, true, 10, 0, "", DataRowVersion.Proposed, _visible));
                command.Parameters.Add(new OracleParameter("P_SSYSTEM", OracleDbType.Int64, 22, ParameterDirection.InputOutput, true, 10, 0, "", DataRowVersion.Proposed, _ssystem));
                command.Parameters.Add(new OracleParameter("P_SECURLEVEL", OracleDbType.Int16, 22, ParameterDirection.InputOutput, true, 2, 0, "", DataRowVersion.Proposed, _securlevel));
                command.Parameters.Add(new OracleParameter("P_CANCHPASSWD", OracleDbType.Int64, 22, ParameterDirection.InputOutput, true, 10, 0, "", DataRowVersion.Proposed, _canchpasswd));
                command.Parameters.Add(new OracleParameter("P_MUSTCHPASSWD", OracleDbType.Int64, 22, ParameterDirection.InputOutput, true, 10, 0, "", DataRowVersion.Proposed, _mustchpasswd));
                command.Parameters.Add(new OracleParameter("P_SESSIONCO", OracleDbType.Int64, 22, ParameterDirection.InputOutput, true, 10, 0, "", DataRowVersion.Proposed, _sessionco));
                command.Parameters.Add(new OracleParameter("P_CARSESCO", OracleDbType.Int64, 22, ParameterDirection.InputOutput, true, 10, 0, "", DataRowVersion.Proposed, _carsesco));
                command.Parameters.Add(new OracleParameter("P_IDLTIM", OracleDbType.Int64, 22, ParameterDirection.InputOutput, true, 10, 0, "", DataRowVersion.Proposed, _idltim));
                command.Parameters.Add(new OracleParameter("P_PASSWORD_BOND", OracleDbType.Varchar2, 100, ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Proposed, _passwordBond));
                command.Parameters.Add(new OracleParameter("P_MECHANIC_PASS", OracleDbType.Varchar2, 200, ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Proposed, _mechanicPass));
                command.Parameters.Add(new OracleParameter("P_INSURANCE_PASS", OracleDbType.Varchar2, 200, ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Proposed, _insurancePass));
                command.Parameters.Add(new OracleParameter("P_IDATE", OracleDbType.Date, 7, ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Proposed, _idate));
                command.Parameters.Add(new OracleParameter("P_IUSER", OracleDbType.Varchar2, 200, ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Proposed, _iuser));
                command.Parameters.Add(new OracleParameter("P_UDATE", OracleDbType.Date, 7, ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Proposed, _udate));
                command.Parameters.Add(new OracleParameter("P_UUSER", OracleDbType.Varchar2, 200, ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Proposed, _uuser));
                command.Parameters.Add(new OracleParameter("P_EMPID", OracleDbType.Varchar2, 10, ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Proposed, _empid));
                command.Parameters.Add(new OracleParameter("P_MAILCODE", OracleDbType.Varchar2, 10, ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Proposed, _mailcode));
                command.Parameters.Add(new OracleParameter("P_POSITION", OracleDbType.Decimal, 22, ParameterDirection.InputOutput, true, 28, 4, "", DataRowVersion.Proposed, _position));
                command.Parameters.Add(new OracleParameter("P_CTY_NB", OracleDbType.Int64, 22, ParameterDirection.InputOutput, true, 10, 0, "", DataRowVersion.Proposed, _ctynb));

                command.ExecuteNonQuery();

                _nb = (Int64?)(decimal?)UnboxOracleParam(command.Parameters["P_NB"].Value);
                _username = (string)UnboxOracleParam(command.Parameters["P_USERNAME"].Value);
                _afirstname = (string)UnboxOracleParam(command.Parameters["P_AFIRSTNAME"].Value);
                _alastname = (string)UnboxOracleParam(command.Parameters["P_ALASTNAME"].Value);
                _password = (string)UnboxOracleParam(command.Parameters["P_PASSWORD"].Value);
                _created = (DateTime?)UnboxOracleParam(command.Parameters["P_CREATED"].Value);
                _gendr = (string)UnboxOracleParam(command.Parameters["P_GENDR"].Value);
                _locked = (string)UnboxOracleParam(command.Parameters["P_LOCKED"].Value);
                _deptnb = (Int64?)(decimal?)UnboxOracleParam(command.Parameters["P_DEPTNB"].Value);
                _prefix = (string)UnboxOracleParam(command.Parameters["P_PREFIX"].Value);
                _visible = (Int64?)(decimal?)UnboxOracleParam(command.Parameters["P_VISIBLE"].Value);
                _ssystem = (Int64?)(decimal?)UnboxOracleParam(command.Parameters["P_SSYSTEM"].Value);
                _securlevel = (Int16?)(decimal?)UnboxOracleParam(command.Parameters["P_SECURLEVEL"].Value);
                _canchpasswd = (Int64?)(decimal?)UnboxOracleParam(command.Parameters["P_CANCHPASSWD"].Value);
                _mustchpasswd = (Int64?)(decimal?)UnboxOracleParam(command.Parameters["P_MUSTCHPASSWD"].Value);
                _sessionco = (Int64?)(decimal?)UnboxOracleParam(command.Parameters["P_SESSIONCO"].Value);
                _carsesco = (Int64?)(decimal?)UnboxOracleParam(command.Parameters["P_CARSESCO"].Value);
                _idltim = (Int64?)(decimal?)UnboxOracleParam(command.Parameters["P_IDLTIM"].Value);
                _passwordBond = (string)UnboxOracleParam(command.Parameters["P_PASSWORD_BOND"].Value);
                _mechanicPass = (string)UnboxOracleParam(command.Parameters["P_MECHANIC_PASS"].Value);
                _insurancePass = (string)UnboxOracleParam(command.Parameters["P_INSURANCE_PASS"].Value);
                _idate = (DateTime?)UnboxOracleParam(command.Parameters["P_IDATE"].Value);
                _iuser = (string)UnboxOracleParam(command.Parameters["P_IUSER"].Value);
                _udate = (DateTime?)UnboxOracleParam(command.Parameters["P_UDATE"].Value);
                _uuser = (string)UnboxOracleParam(command.Parameters["P_UUSER"].Value);
                _empid = (string)UnboxOracleParam(command.Parameters["P_EMPID"].Value);
                _mailcode = (string)UnboxOracleParam(command.Parameters["P_MAILCODE"].Value);
                _position = (decimal?)UnboxOracleParam(command.Parameters["P_POSITION"].Value);
                _ctynb = (Int64?)(decimal?)UnboxOracleParam(command.Parameters["P_CTY_NB"].Value);

            }
            catch (Exception ex)
            {
                throw new Exception("DAOAppmgrUsers::Error Occurred:Update:" + ex.Message, ex);
            }
            finally
            {
                command.Dispose();
            }
        }

        #endregion

        #region member properties
        public Int64? Nb
        {
            get
            {
                return _nb;
            }
            set
            {
                _nb = value;
            }
        }
        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
            }
        }
        public string Afirstname
        {
            get
            {
                return _afirstname;
            }
            set
            {
                _afirstname = value;
            }
        }
        public string Alastname
        {
            get
            {
                return _alastname;
            }
            set
            {
                _alastname = value;
            }
        }
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
            }
        }
        public DateTime? Created
        {
            get
            {
                return _created;
            }
            set
            {
                _created = value;
            }
        }
        public string Gendr
        {
            get
            {
                return _gendr;
            }
            set
            {
                _gendr = value;
            }
        }
        public string Locked
        {
            get
            {
                return _locked;
            }
            set
            {
                _locked = value;
            }
        }
        public Int64? Deptnb
        {
            get
            {
                return _deptnb;
            }
            set
            {
                _deptnb = value;
            }
        }
        public string Prefix
        {
            get
            {
                return _prefix;
            }
            set
            {
                _prefix = value;
            }
        }
        public Int64? Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                _visible = value;
            }
        }
        public Int64? Ssystem
        {
            get
            {
                return _ssystem;
            }
            set
            {
                _ssystem = value;
            }
        }
        public Int16? Securlevel
        {
            get
            {
                return _securlevel;
            }
            set
            {
                _securlevel = value;
            }
        }
        public Int64? Canchpasswd
        {
            get
            {
                return _canchpasswd;
            }
            set
            {
                _canchpasswd = value;
            }
        }
        public Int64? Mustchpasswd
        {
            get
            {
                return _mustchpasswd;
            }
            set
            {
                _mustchpasswd = value;
            }
        }
        public Int64? Sessionco
        {
            get
            {
                return _sessionco;
            }
            set
            {
                _sessionco = value;
            }
        }
        public Int64? Carsesco
        {
            get
            {
                return _carsesco;
            }
            set
            {
                _carsesco = value;
            }
        }
        public Int64? Idltim
        {
            get
            {
                return _idltim;
            }
            set
            {
                _idltim = value;
            }
        }
        public string PasswordBond
        {
            get
            {
                return _passwordBond;
            }
            set
            {
                _passwordBond = value;
            }
        }
        public string MechanicPass
        {
            get
            {
                return _mechanicPass;
            }
            set
            {
                _mechanicPass = value;
            }
        }
        public string InsurancePass
        {
            get
            {
                return _insurancePass;
            }
            set
            {
                _insurancePass = value;
            }
        }
        public DateTime? Idate
        {
            get
            {
                return _idate;
            }
            set
            {
                _idate = value;
            }
        }
        public string Iuser
        {
            get
            {
                return _iuser;
            }
            set
            {
                _iuser = value;
            }
        }
        public DateTime? Udate
        {
            get
            {
                return _udate;
            }
            set
            {
                _udate = value;
            }
        }
        public string Uuser
        {
            get
            {
                return _uuser;
            }
            set
            {
                _uuser = value;
            }
        }
        public string Empid
        {
            get
            {
                return _empid;
            }
            set
            {
                _empid = value;
            }
        }
        public string Mailcode
        {
            get
            {
                return _mailcode;
            }
            set
            {
                _mailcode = value;
            }
        }
        public decimal? Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }
        public Int64? Ctynb
        {
            get
            {
                return _ctynb;
            }
            set
            {
                _ctynb = value;
            }
        }
        #endregion
    }
    public partial class AppMgr_TxConnectionProvider : IDisposable
    {
        protected bool _isDisposed;
        protected OracleConnection _txConnection;
        protected OracleTransaction _currTransaction;
        static string _connectionString;

        public AppMgr_TxConnectionProvider()
        {
            Init();
        }

        private void Init()
        {
            _txConnection = new OracleConnection();
            _txConnection.ConnectionString = ConnectionString;
            _currTransaction = null;
            _isDisposed = false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!_isDisposed)
            {
                if (isDisposing)
                {
                    if (_currTransaction != null)
                    {
                        _currTransaction.Dispose();
                        _currTransaction = null;
                    }
                    if (_txConnection != null)
                    {
                        /*this will also rollback any pending transactions on this connection*/
                        _txConnection.Close();
                        _txConnection.Dispose();
                        _txConnection = null;
                    }
                }
            }
            _isDisposed = true;
        }

        public virtual void OpenConnection()
        {
            try
            {
                if (_txConnection.State == ConnectionState.Open)
                    throw new Exception("Connection is already open");

                _txConnection.Open();
                _currTransaction = null;
                _isDisposed = false;
            }
            catch
            {
                throw;
            }
        }

        public virtual void CloseConnection(bool commit)
        {
            if ((_txConnection == null) || (_txConnection.State != ConnectionState.Open))
                return;
            try
            {
                if ((_currTransaction != null) && commit)
                    _currTransaction.Commit();

                else if (_currTransaction != null)
                    _currTransaction.Rollback();

                if (_currTransaction != null)
                    _currTransaction.Dispose();

                _currTransaction = null;
                _txConnection.Close();
            }
            catch
            {
                throw;
            }
        }

        public virtual void BeginTransaction(string trans)
        {
            if (_currTransaction != null)
                throw new Exception("Transaction nesting not allowed");

            if ((_txConnection == null) || (_txConnection.State != ConnectionState.Open))
                throw new Exception("Connection not open");

            try
            {
                _currTransaction = _txConnection.BeginTransaction(IsolationLevel.ReadCommitted);
                _currTransaction.Save(trans);
            }
            catch
            {
                throw;
            }
        }

        public virtual void CommitTransaction()
        {
            if (_currTransaction == null)
                throw new Exception("No Transaction to commit");

            if ((_txConnection == null) || (_txConnection.State != ConnectionState.Open))
                throw new Exception("Connection not open");

            try
            {
                _currTransaction.Commit();
                _currTransaction.Dispose();
                _currTransaction = null;
            }
            catch
            {
                throw;
            }
        }

        public virtual void RollbackTransaction(string trans)
        {
            if (_currTransaction == null)
                throw new Exception("No Transaction to rollback");

            if ((_txConnection == null) || (_txConnection.State != ConnectionState.Open))
                throw new Exception("Connection not open");

            try
            {
                _currTransaction.Rollback(trans);
                _currTransaction.Dispose();
                _currTransaction = null;
            }
            catch
            {
                throw;
            }
        }

        public virtual OracleTransaction CurrentTransaction
        {
            get
            {
                return _currTransaction;
            }
        }

        public virtual OracleConnection Connection
        {
            get
            {
                return _txConnection;
            }
        }

        public static string ConnectionString
        {
            set { _connectionString = value; }
            get
            {
                if (!string.IsNullOrEmpty(_connectionString))
                    return _connectionString;

                return System.Configuration.ConfigurationManager.ConnectionStrings["DrivingSclEntity"].ConnectionString;
            }
        }
    }
    public partial class AppMgrConn : IDisposable
    {
        #region members
        protected AppMgr_TxConnectionProvider _connectionProvider;
        static string _staticConnectionString;
        bool _isDisposed = false;
        #endregion

        #region initialisation
        public AppMgrConn()
        {
            Init();
        }

        private void Init()
        {
        }
        #endregion

        #region connection support
        public virtual AppMgr_TxConnectionProvider ConnectionProvider
        {
            set
            {
                if (value == null)
                    throw new Exception("Connection provider cannot be null");

                _connectionProvider = value;
            }
        }

        public static OracleConnection StaticSqlConnection
        {
            get
            {
                OracleConnection staticConnection = new OracleConnection();
                staticConnection.ConnectionString = StaticConnectionString;
                return staticConnection;
            }
        }

        public static string StaticConnectionString
        {
            set { _staticConnectionString = value; }
            get
            {
                if (!string.IsNullOrEmpty(_staticConnectionString))
                    return _staticConnectionString;
                return System.Configuration.ConfigurationManager.ConnectionStrings["DrivingSclEntity"].ConnectionString;
            }
        }
        #endregion

        #region disposable interface support
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!_isDisposed)
            {
                if (isDisposing)
                {
                    if (_connectionProvider != null)
                    {
                        ((IDisposable)_connectionProvider).Dispose();
                        _connectionProvider = null;
                    }
                }
            }
            _isDisposed = true;
        }
        #endregion

        #region oracle parameter helpers
        protected virtual object UnboxOracleParam(object outParam)
        {
            Type t = outParam.GetType();
            if (t == typeof(OracleDecimal))
            {
                if (!((OracleDecimal)outParam).IsNull)
                    return (decimal)((OracleDecimal)outParam).Value;
                else
                    return null;
            }
            else if (t == typeof(OracleString))
            {
                if (!((OracleString)outParam).IsNull)
                    return (string)((OracleString)outParam).Value;
                else
                    return null;
            }
            else if (t == typeof(OracleIntervalYM))
            {
                if (!((OracleIntervalYM)outParam).IsNull)
                    return (Int64)((OracleIntervalYM)outParam).Value;
                else
                    return null;
            }
            else if (t == typeof(OracleIntervalDS))
            {
                if (!((OracleIntervalDS)outParam).IsNull)
                    return (TimeSpan)((OracleIntervalDS)outParam).Value;
                else
                    return null;
            }
            else if (t == typeof(OracleBinary))
            {
                if (!((OracleBinary)outParam).IsNull)
                    return (byte[])((OracleBinary)outParam).Value;
                else
                    return null;
            }
            else if (t == typeof(OracleDate))
            {
                if (!((OracleDate)outParam).IsNull)
                    return (DateTime)((OracleDate)outParam).Value;
                else
                    return null;
            }
            else if (t == typeof(OracleTimeStamp))
            {
                if (!((OracleTimeStamp)outParam).IsNull)
                    return (DateTime)((OracleTimeStamp)outParam).Value;
                else
                    return null;
            }
            else if (t == typeof(OracleBlob))
            {
                if (!((OracleBlob)outParam).IsNull)
                    return (byte[])((OracleBlob)outParam).Value;
                else
                    return null;
            }
            return outParam;
        }
        #endregion

    }
    public partial class AppsManagerEntities : DbContext
    {
        public AppsManagerEntities()
            : base(AppMgrConn.StaticConnectionString)
        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
            /*modelBuilder.HasDefaultSchema("NOTARY");
            base.OnModelCreating(modelBuilder);*/
        }
        public virtual DbSet<DAOUsers> USERS { get; set; }
    }

    public class UserAccountManager
    {
        Claim[] rolesClaims = null;

        private AppsManagerEntities db = new AppsManagerEntities();

        public UserAccountManager(AppsManagerEntities dbContext)
        {
            this.db = dbContext;
        }

        private bool IsValidCredintials(LoginViewModel loginModel)
        {
            var ph = new PasswordHasher();
            //string pwd = ph.HashPassword(loginModel.Password);
            var user = new DAOUsers();
            try
            {
                user.Username = loginModel.UserName.ToLower();
                user = DAOUsers.SelectAllBySearchFields(user).SingleOrDefault(u => u.Username.ToLower() == loginModel.UserName.ToLower() && u.Locked != "0");
            }
            catch (Exception ex)
            {
                throw;
            }
            if (user == null)
                return false;
            var res = ph.VerifyHashedPassword(user.Password, loginModel.Password);
            if (user != null && (res == PasswordVerificationResult.Success || res == PasswordVerificationResult.SuccessRehashNeeded))
            {
                var Roles = DAOUserRoles.SelectAllByUsernb(user.Nb);
                int length = Roles.Count;
                if (rolesClaims == null)
                {
                    rolesClaims = new Claim[length];
                }
                for (int i = 0; i < length; i++)
                {
                    rolesClaims[i] = new Claim(ClaimTypes.Role, Roles[i].Nb.ToString());
                }
                return true;
            }
            return false;
        }

        public SignInStatus Login(LoginViewModel loginModel)
        {
            if (this.IsValidCredintials(loginModel))
            {
                var ident = new ClaimsIdentity(
                  new[]
                  {
                      // adding following 2 claim just for supporting default antiforgery provider
                      new Claim(ClaimTypes.NameIdentifier, loginModel.UserName),
                      new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),
                      new Claim(ClaimTypes.Name, loginModel.UserName),
                  },
                  DefaultAuthenticationTypes.ApplicationCookie);
                ident.AddClaims(rolesClaims);
                HttpContext.Current.GetOwinContext().Authentication.SignIn(new AuthenticationProperties { IsPersistent = false }, ident);

                return SignInStatus.Success; // auth succeed 
            }

            return SignInStatus.Failure;
        }
    }
    public class LoginViewModel
    {
        [Required(ErrorMessage = "لا يمكن أن يكون اسم المستخدم فارغاً")]
        [Display(Name = "اسم المستخدم")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "لا يمكن أن تكون كلمة المرور فارغة")]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور")]
        public string Password { get; set; }

        [Display(Name = "تذكّر تسجيل الدخول")]
        public bool RememberMe { get; set; }
    }
    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
    public class ChangePasswordViewModel
    {
        [Display(Name = "كلمة المرور السابقة")]
        public string OldPassword { get; set; }

        [Display(Name = "كلمة المرور الجديدة")]
        public string NewPassword { get; set; }

        [Compare("NewPassword")]
        [Display(Name = "إعادة إدخال كلمة المرور الجديدة")]
        public string NewPasswordConfirmed { get; set; }

        [Display(Name = "عدد السجلات في الصفحة")]
        public int DefaultRecordsPerPage { get; set; }

        [Display(Name = "خيار الطباعة الافتراضي")]
        public int DefaultPrintOption { get; set; }

        public int Type { get; set; }
    }
}
