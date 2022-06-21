using Microsoft.AspNet.Identity;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web.Mvc;
using System.Xml;

namespace DrivingSclApp
{
    public static class MyDataBase
    {

        public static string RunQuery_reader(string queryStr)
        {
            string retValue = "";
            object ret;
            OracleConnection conn = new OracleConnection(MyDataBase.GetConnectionString());
            try
            {

                OracleCommand cmd = new OracleCommand(queryStr, conn);
                conn.Open();
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    retValue = dr.GetValue(0).ToString();
                }
                else
                {
                    retValue = "0";
                }
            }
            catch (Exception e)
            {
                retValue = "0";
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }



            return retValue;
        }

        public static double ExecQueryRtNB(string queryStr)
        {
            double cnt = 0;
            OracleConnection conn = null;
            try
            {
                // conn = new OracleConnection(GetConnectionString());
                conn = new OracleConnection(GetConnectionString());
                conn.Open();
                OracleCommand oraCommand = new OracleCommand(queryStr, conn);
                //  oraCommand.Parameters.Add(new OracleParameter("insertedNB", cnt));
                oraCommand.Parameters.Add(":insertedNB", OracleDbType.Int32, 10, cnt, ParameterDirection.ReturnValue);


                //if (oraReader.HasRows)
                //{
                //    while (oraReader.Read())
                //    {
                //        fullName = oraReader.GetString(0);
                //    }
                //}


                //OracleCommand cmd = new OracleCommand(queryStr, conn);
                oraCommand.ExecuteNonQuery();
                cnt = double.Parse(oraCommand.Parameters[":insertedNB"].Value.ToString());

            }
            catch { }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return cnt;
        }
        public static SelectList GetPrintOptionList(string defaultValue)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "طباعة وورد Word", Value = "1" });
            items.Add(new SelectListItem { Text = "طباعة إكسل Excel", Value = "2" });
            items.Add(new SelectListItem { Text = "طباعة Html", Value = "3" });
            return new SelectList(items, "Value", "Text", string.IsNullOrEmpty(defaultValue) ? "3" : defaultValue);
        }

        public static SelectList GetDBList(string defaultValue)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            XmlDocument xmlDoc = new XmlDocument();
            string path = @"C:\inetpub\wwwroot\TNSName.XML";
            try
            {
                xmlDoc.PreserveWhitespace = true;
                xmlDoc.Load(path);
                XmlNodeList elemList1 = xmlDoc.GetElementsByTagName("Name");
                XmlNodeList elemList2 = xmlDoc.GetElementsByTagName("Oracle");
                for (int i = 0; i < elemList1.Count; i++)
                {
                    items.Add(new SelectListItem
                    {
                        Text = elemList1[i].InnerText,
                        Value = elemList2[i].InnerText
                    });
                }
            }
            catch
            {
                items.Add(new SelectListItem { Text = "قاعدة المعطيات", Value = "Oracle" });
            }
            return new SelectList(items, "Value", "Text", string.IsNullOrEmpty(defaultValue) ? "Oracle" : defaultValue);
        }
        public static int GetRowsPerPage(Int64? id)
        {
            string retValue = "20";
            OracleConnection conn = new OracleConnection(MyDataBase.GetConnectionString());
            try
            {
                string pNb = RunQuery("SELECT NB FROM APPMGR.PROGRAMS WHERE NAME='" + Resource.ProgID + "'");
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT VAL FROM AppMgr.SETTINGS WHERE NAME like '%ROWS_PER_PAGE%'  AND USERNB='" + id + "' and prognb=" + pNb, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    retValue = dr.GetDouble(0).ToString();
                }
                else
                {
                    retValue = "20";
                }
            }
            catch
            {
                retValue = "7";
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return int.Parse(retValue);
        }
        public static string GetEmpFullName(long? id)
        {
            string retValue = "";
            OracleConnection conn = new OracleConnection(MyDataBase.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM AppMgr.USERS WHERE NB='" + id + "'", conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    string UserNB = dr.GetValue(0).ToString();
                    if (!dr.IsDBNull(6))
                        retValue = dr.GetValue(6).ToString() == "ذكر" ? "السيد " : "السيدة ";
                    retValue += dr.GetString(2) + " " + dr.GetString(3);
                }
            }
            catch
            {
                retValue = "";
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return retValue;
        }
        public static void SetUserPassword(double nb, string pwd)
        {
            var ph = new PasswordHasher();
            string sql2 = "Update AppMgr.Users set PASSWORD = '" + ph.HashPassword(pwd) + "' WHERE NB=" + nb;
            MyDataBase.ExecQuery(sql2);
        }
        public static string GetConnectionString()
        {
            //return "Data Source=ORACLE;Persist Security Info=True;User ID=GOVECOMPLAIN;Password=g";
            return System.Configuration.ConfigurationManager.ConnectionStrings["DrivingSclEntity"].ConnectionString;
        }
        public static string Connect(string usrName, string pwd)
        {
            string retValue = "";
            OracleConnection conn = new OracleConnection(MyDataBase.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM AppMgr.USERS WHERE UserName='" + usrName + "'", conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    var ph = new PasswordHasher();

                    string UPwd = dr.GetString(4);

                    var res = ph.VerifyHashedPassword(UPwd, pwd);

                    bool ok = (res == PasswordVerificationResult.Success || res == PasswordVerificationResult.SuccessRehashNeeded);

                    if (ok)
                    {
                        retValue = dr.GetValue(0).ToString();
                    }
                    else
                    {
                        retValue = "";
                    }
                }
            }
            catch (Exception e)
            {
                retValue = e.Message;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return retValue;
        }
        public static double ExecQuery(string queryStr)
        {
            double cnt = 0;
            OracleConnection conn = null;
            try
            {
                conn = new OracleConnection(GetConnectionString());
                conn.Open();
                OracleCommand cmd = new OracleCommand(queryStr, conn);
                cnt = cmd.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return cnt;
        }
        public static string RunQuery(string queryStr)
        {
            string retValue = "";
            object ret;
            OracleConnection conn = new OracleConnection(GetConnectionString());
            using (conn)
            {
                conn.Open();
                OracleCommand cmd = conn.CreateCommand();
                cmd.CommandText = queryStr;
                ret = cmd.ExecuteScalar();
                if (ret != null)
                    retValue = ret.ToString();
                conn.Close();
                conn.Dispose();
            }
            return retValue;
        }
        public static string GetNameFromIdValue(string tbl, string fid, string fname, string vid)
        {
            OracleConnection conn = new OracleConnection(MyDataBase.GetConnectionString());
            conn.Open();
            string queryStr = "SELECT " + fname + " FROM " + tbl + " WHERE " + fid + " = '" + vid + "'";
            string retValue = "";
            OracleCommand sds = new OracleCommand(queryStr, conn);
            try
            {
                retValue = sds.ExecuteScalar().ToString();
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return retValue;
        }
        public static string GetIDFromNameValue(string tbl, string fname, string fid, string vname)
        {
            OracleConnection conn = new OracleConnection(MyDataBase.GetConnectionString());
            conn.Open();
            string queryStr = "SELECT " + fid + " FROM " + tbl + " WHERE " + fname + " = '" + vname + "'";
            string retValue = "";
            OracleCommand sds = new OracleCommand(queryStr, conn);
            try
            {
                retValue = sds.ExecuteScalar().ToString();
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return retValue;
        }
        public static Int64 GetSeqValue(string seqName)
        {
            OracleConnection conn = new OracleConnection(MyDataBase.GetConnectionString());
            conn.Open();
            string queryStr = "SELECT " + seqName + ".NEXTVAL FROM DUAL";
            string retValue = "";
            OracleCommand sds = new OracleCommand(queryStr, conn);
            try
            {
                retValue = sds.ExecuteScalar().ToString();
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return long.Parse(retValue);
        }
        public static SelectList GetBontTypesList(string defaultValue)
        {
            OracleConnection conn = new OracleConnection(GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmdCnt = conn.CreateCommand();
                cmdCnt.CommandText = "SELECT COUNT(*) FROM BONDTYPES ";
                List<SelectListItem> items = new List<SelectListItem>();
                OracleCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM BONDTYPES ORDER BY NB ";
                OracleDataReader dr = cmd.ExecuteReader();
                items.Add(new SelectListItem { Text = "الكل", Value = 0.ToString() });
                while (dr.Read())
                {
                    items.Add(new SelectListItem { Text = dr.GetString(1), Value = dr.GetValue(0).ToString() });
                }
                conn.Close();
                conn.Dispose();
                return new SelectList(items, "Value", "Text", string.IsNullOrEmpty(defaultValue) ? "0" : defaultValue);
            }
            catch
            {
                conn.Close();
                conn.Dispose();
                return null;
            }
        }



        public static SelectList GetSourceList(string defaultValue)
        {
            OracleConnection conn = new OracleConnection(GetConnectionString());
            try
            {
                conn.Open();
                List<SelectListItem> items = new List<SelectListItem>();
                OracleCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM COD_SOURCE ORDER BY ID ";
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    items.Add(new SelectListItem { Text = dr.GetString(1), Value = dr.GetValue(0).ToString() });
                }
                conn.Close();
                conn.Dispose();
                return new SelectList(items, "Value", "Text", string.IsNullOrEmpty(defaultValue) ? "0" : defaultValue);
            }
            catch
            {
                conn.Close();
                conn.Dispose();
                return null;
            }
        }



        public static SelectList GetFactoryList(string defaultValue)
        {
            OracleConnection conn = new OracleConnection(GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmdCnt = conn.CreateCommand();
                cmdCnt.CommandText = "SELECT COUNT(*) FROM BRANCHES ";
                List<SelectListItem> items = new List<SelectListItem>();
                OracleCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM BRANCHES ORDER BY NB ";
                OracleDataReader dr = cmd.ExecuteReader();
                items.Add(new SelectListItem { Text = "الكل", Value = "-1" });
                while (dr.Read())
                {
                    items.Add(new SelectListItem { Text = dr.GetString(1), Value = dr.GetValue(0).ToString() });
                }
                conn.Close();
                conn.Dispose();
                return new SelectList(items, "Value", "Text", string.IsNullOrEmpty(defaultValue) ? "0" : defaultValue);
            }
            catch
            {
                conn.Close();
                conn.Dispose();
                return null;
            }
        }

        public static string ExePorc(string ProcName, List<string> Params)
        {
            string retValue = "";
            string listparams = "";
            object ret;
            OracleConnection conn = new OracleConnection(GetConnectionString());
            using (conn)
            {
                conn.Open();
                OracleCommand cmd = conn.CreateCommand();

                foreach (string param in Params)
                {

                    listparams += ", " + param;

                }

                cmd.CommandText = "begin " + ProcName + "( " + listparams.Remove(0, 1) + "); end; ";
                ret = cmd.ExecuteScalar();
                if (ret != null)
                    retValue = ret.ToString();
                conn.Close();
                conn.Dispose();
            }
            return retValue;
        }
        public static int GetReportType(Int64? id)
        {
            string retValue = "3";
            OracleConnection conn = new OracleConnection(GetConnectionString());
            try
            {
                string pNb = MyDataBase.RunQuery("SELECT NB FROM APPMGR.PROGRAMS WHERE NAME='" + Resource.ProgID + "'");
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT VAL FROM AppMgr.SETTINGS WHERE name like  '%REPORT_TYPE%'  AND USERNB='" + id + "' and ProgNb=" + pNb, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    retValue = dr.GetString(0);
                }
                else
                {
                    retValue = "1";
                }
            }
            catch
            {
                retValue = "1";
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return int.Parse(retValue);
        }
        public static string Getmaxnum(string nb, string tablename, string wherstr)
        {
            OracleConnection conn = new OracleConnection(MyDataBase.GetConnectionString());
            conn.Open();
            string queryStr = "select nvl(max(" + nb + "),0) +1   from CONTRACT." + tablename + "" + " Where 1=1 " + wherstr;
            string retValue = "";
            OracleCommand sds = new OracleCommand(queryStr, conn);
            try
            {
                retValue = sds.ExecuteScalar().ToString();
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return retValue;
        }

        public static string GetValue(double? str)
        {
            if (!str.HasValue)
                return "";
            else
                return str.Value.ToString();
        }
        public static string GetValue(DateTime? str, [DefaultValue(false)]bool dty)
        {
            if (!str.HasValue)
                return "";
            else if (dty)
                return str.Value.ToString("yyyy/MM/yy");
            else
                return str.Value.ToString("dd/MM/yyyy");
        }
        public static string GetValue(DateTime str)
        {
            return str.ToString("dd/MM/yyyy");
        }
        public static string GetValue(string str)
        {
            if (string.IsNullOrEmpty(str))
                return "";
            else
                return str;
        }
    }

}
