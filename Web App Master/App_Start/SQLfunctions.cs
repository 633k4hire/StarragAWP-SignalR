using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using Helpers;
using System.Threading.Tasks;

public class _SQLerror
    {
        public _SQLerror() { }
        public Exception Ex = null;
        public string ErrorMsg = "generic";
        public object Tag;
    }
    public class SQL_Request
    {
    
        public bool Success = false;
        public object Tag;
        public _SQLerror Error = new _SQLerror();
        public DataSet Data = null;
        public string Message = "default";
        public SqlConnection Connection;
        public SqlCommand Command;
    }

public static class SQLfunc
{
    public static Exception _LastSqlException = null;

    //public static string _ConnectionString = @"Server=tcp:lastsoul.database.windows.net,1433;Initial Catalog=db;Persist Security Info=False;User ID=yuriebasuta;Password=Gh0stbust3r;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
    //public static string _ConnectionString = @"Data Source=lastsoul.database.windows.net;Initial Catalog=db;Integrated Security=False;User ID=yuriebasuta;Password=Gh0stbust3r;Connect Timeout=60;Encrypt=True;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"; // @"Data Source=lastsoul.database.windows.net;Initial Catalog=db;Integrated Security=False;User ID=yuriebasuta;Password=********;Connect Timeout=60;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
    public static string _ConnectionString = @"Server=tcp:lastsoul.database.windows.net,1433;Initial Catalog=db;Persist Security Info=False;User ID=yuriebasuta;Password=Gh0stbust3r;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=60;";
    public static SQL_Request OpenConnection(this SQL_Request request, string connectionString = null)
    {

        try
        {
            if (connectionString == null)
                connectionString = _ConnectionString;
            request.Connection = new SqlConnection(connectionString);
            request.Success = true;
            request.Connection.Open();
        }
        catch (Exception ex)
        {
            request.Error.Ex = ex;
            request.Success = false;
        }
        return request;
    }
    public static async Task<SQL_Request> OpenConnectionAsync(this SQL_Request request, string connectionString = null)
    {
        try
        {
            if (connectionString == null)
                connectionString = _ConnectionString;
            request.Connection = new SqlConnection(connectionString);
            request.Success = true;
            await request.Connection.OpenAsync();
        }
        catch (Exception ex)
        {
            request.Error.Ex = ex;
            request.Success = false;
        }
        return request;
    }
    public static SQL_Request CloseConnection(this SQL_Request request)
    {

        try
        {
            request.Connection.Close();
            request.Command.Dispose();
            request.Success = true;
        }
        catch (Exception ex)
        {
            request.Error.Ex = ex;
            request.Success = false;
        }
        return request;
    }
    //Asset
    public static SQL_Request AddAsset(this SQL_Request request, Asset asset, bool close = true)
    {
        if (request.Connection.State == ConnectionState.Closed)
        { request.Connection.Open(); }
        request.Command = new SqlCommand();

        try
        {
            string imgs = "";
           
            imgs = asset.Images;
            if (request.Connection.State == ConnectionState.Closed)
            { request.Connection.Open(); }
            request.Command = new SqlCommand();
            request.Command.Connection = request.Connection; //Pass the connection object to Command
            request.Command.CommandType = CommandType.StoredProcedure; // We will use stored procedure.
            request.Command.CommandText = "AssetInsert"; //Stored Procedure Name

            //	 @AssetName,@AssetNumber,@CalibratedAsset,@Damaged,@OnHold,@BarcodeImage,
            //@CalibrationCompany,@CalibrationHistory,@CalibrationPeriod,@DateReturned,
            //@DateShipped,@AssetDescription,@LastCalibrated,@OrderNumber,@PersonShipping,
            //@PackingSlip,@ReturnReport,@UPSlabel,@Images,@ImageLinks,@ServiceEngineer,@ShipTo,@AssetWeight
            request.Command.Parameters.Add("@AssetName", SqlDbType.NVarChar).Value = asset.AssetName;
            request.Command.Parameters.Add("@AssetNumber", SqlDbType.NVarChar).Value = asset.AssetNumber;
            request.Command.Parameters.Add("@CalibratedAsset", SqlDbType.Bit).Value = asset.IsCalibrated;
            request.Command.Parameters.Add("@Damaged", SqlDbType.Bit).Value = asset.IsDamaged;
            request.Command.Parameters.Add("@OnHold", SqlDbType.Bit).Value = asset.OnHold;
            request.Command.Parameters.Add("@IsOut", SqlDbType.Bit).Value = asset.IsOut;
            request.Command.Parameters.Add("@BarcodeImage", SqlDbType.NVarChar).Value = asset.BarcodeImage; //FIX
            request.Command.Parameters.Add("@CalibrationCompany", SqlDbType.NVarChar).Value = asset.CalibrationCompany;
            string calXml = new CalibrationLibrary().SerializeToXmlString(new CalibrationLibrary());
            try
            {
                calXml = asset.CalibrationHistory.SerializeToXmlString(asset.CalibrationHistory);
            }
            catch { }
            request.Command.Parameters.Add("@CalibrationHistory", SqlDbType.NVarChar).Value = "";
            request.Command.Parameters.Add("@CalibrationPeriod", SqlDbType.NVarChar).Value = asset.CalibrationPeriod;
            request.Command.Parameters.Add("@DateReturned", SqlDbType.NVarChar).Value = asset.DateRecieved.ToString();
            request.Command.Parameters.Add("@DateShipped", SqlDbType.NVarChar).Value = asset.DateShipped.ToString();
            request.Command.Parameters.Add("@AssetDescription", SqlDbType.NVarChar).Value = asset.Description;
            request.Command.Parameters.Add("@LastCalibrated", SqlDbType.NVarChar).Value = asset.LastCalibrated.ToString();
            request.Command.Parameters.Add("@OrderNumber", SqlDbType.NVarChar).Value = asset.OrderNumber.ToString();
            request.Command.Parameters.Add("@PersonShipping", SqlDbType.NVarChar).Value = asset.PersonShipping;
            request.Command.Parameters.Add("@Images", SqlDbType.NVarChar).Value = imgs;
            request.Command.Parameters.Add("@ImageLinks", SqlDbType.NVarChar).Value = imgs;
            request.Command.Parameters.Add("@ServiceEngineer", SqlDbType.NVarChar).Value = asset.ServiceEngineer;
            request.Command.Parameters.Add("@ShipTo", SqlDbType.NVarChar).Value = asset.ShipTo;
            request.Command.Parameters.Add("@AssetWeight", SqlDbType.NVarChar).Value = asset.weight.ToString();
            try
            {
                foreach (var ii in asset.History.History)
                {
                    ii.IsHistoryItem = true;
                }
            }
            catch { }
            var histxml = asset.History.Serialize();
            request.Command.Parameters.Add("@History", SqlDbType.NVarChar).Value = histxml;
            request.Command.Parameters.Add("@PackingSlip", SqlDbType.NVarChar).Value = asset.PackingSlip;
            request.Command.Parameters.Add("@ReturnReport", SqlDbType.NVarChar).Value = asset.ReturnReport;
            request.Command.Parameters.Add("@UPSlabel", SqlDbType.NVarChar).Value = asset.UpsLabel;
            string doc_csv = "";
            if (asset.Documents != null)
            {
                if (asset.Documents.Count > 0)
                {
                    foreach (var item in asset.Documents)
                    {
                        doc_csv += item + ",";
                    }
                }
            }
            request.Command.Parameters.Add("@Documents", SqlDbType.NVarChar).Value = doc_csv;
            request.Command.ExecuteNonQuery();
            request.Success = true;
            request.Message = "success:addAsset";

        }
        catch (Exception ex)
        {
            request.Error.Ex = ex;
            request.Success = false;
            request.Message = "error:addAsset";
            return request;
        }
        finally
        {
            if (close)
                CloseConnection(request);
        }
        request.Message = "success:addAsset";
        request.Success = true;
        return request;
    }
    public static SQL_Request GetAssets(this SQL_Request request, bool close = true)
    {
        request.Command = new SqlCommand();

        try
        {
            if (request.Connection.State == ConnectionState.Closed)
            { request.Connection.Open(); }
            // Create a object of SqlCommand class
            request.Command.Connection = request.Connection; //Pass the connection object to Command
            request.Command.CommandType = CommandType.StoredProcedure; // We will use stored procedure.

            request.Command.CommandText = "AssetGetAll";
            //request.Command.CommandText = "spGetTimeData"; //Stored Procedure Name
            //request.Command.Parameters.Add("@AssetNumber", SqlDbType.NVarChar).Value = AssetNumber;

            SqlDataAdapter da = new SqlDataAdapter(request.Command);
            DataSet ds = new DataSet();
            da.Fill(ds);
            request.Message = "success:getalldata";
            request.Data = ds;
            request.Success = true;
            List<Asset> newassets = new List<Asset>();
            try
            {
               
                foreach (DataRow dr in request.Data.Tables[0].Rows)
                {
                    Asset a = new Asset();
                    a.AssetName = dr?.Field<string>("AssetName");
                    a.AssetNumber = dr?.Field<string>("AssetNumber");
                    try
                    {
                        a.OrderNumber = dr?.Field<string>("OrderNumber");
                    }
                    catch { }
                    a.ShipTo = dr?.Field<string>("ShipTo");
                    a.IsOut= dr.Field<bool>("IsOut");
                    a.DateShipped = DateTime.Parse(dr?.Field<string>("DateShipped"));
                    a.ServiceEngineer = dr?.Field<string>("ServiceEngineer");
                    a.PersonShipping = dr?.Field<string>("PersonShipping");
                    a.DateRecieved = DateTime.Parse(dr?.Field<string>("DateReturned"));
                    a.weight = Convert.ToDecimal(dr?.Field<string>("AssetWeight"));
                    a.IsDamaged = dr.Field<bool>("Damaged");
                    a.OnHold = dr.Field<bool>("OnHold");
                    a.Description = dr?.Field<string>("AssetDescription");
                    a.IsCalibrated = dr.Field<bool>("CalibratedAsset");
                    a.CalibrationCompany = dr?.Field<string>("CalibrationCompany");
                    a.LastCalibrated = DateTime.Parse(dr?.Field<string>("LastCalibrated"));
                    a.CalibrationPeriod = dr?.Field<string>("CalibrationPeriod");
                    var calXml = dr?.Field<string>("CalibrationHistory");
                    try
                    {
                        a.CalibrationHistory = new CalibrationLibrary().DeserializeFromXmlString<CalibrationLibrary>(calXml);
                    }
                    catch { }
                 
                    a.Images = dr?.Field<string>("Images");
                    a.BarcodeImage = dr?.Field<string>("BarcodeImage");
                    try
                    {
                        var xml = dr?.Field<string>("History");
                        a.History = new AssetHistory().Deserialize(xml);
                        foreach(var ii in a.History.History)
                        {
                            ii.IsHistoryItem = true;
                        }
                    }
                    catch { }
                    a.PackingSlip = dr?.Field<string>("PackingSlip");
                    a.UpsLabel = dr?.Field<string>("UpsLabel");
                    a.ReturnReport = dr?.Field<string>("ReturnReport");
                    var doc_csv = dr?.Field<string>("Documents");
                    a.Documents = new List<string>();
                    if (doc_csv!=null)
                    {
                        if (doc_csv!="")
                        {
                            a.Documents = doc_csv.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        }
                    }
                   
                    newassets.Add(a);
                }
                request.Tag = newassets;
            }
            catch { }

        }
        catch (Exception ex)
        {
            request.Error.Ex = ex;
            request.Success = false;
            request.Message = "error:getalldata";
            return request;
        }
        finally
        {
            if (close)
                CloseConnection(request);
        }
        return request;
    }
    public static SQL_Request GetAsset(this SQL_Request request, string AssetNumber, bool close = true)
    {
        request.Command = new SqlCommand();

        try
        {
            if (request.Connection.State == ConnectionState.Closed)
            { request.Connection.Open(); }
            // Create a object of SqlCommand class
            request.Command.Connection = request.Connection; //Pass the connection object to Command
            request.Command.CommandType = CommandType.StoredProcedure; // We will use stored procedure.

            request.Command.CommandText = "AssetGet";
            //request.Command.CommandText = "spGetTimeData"; //Stored Procedure Name
            
            request.Command.Parameters.Add("@AssetNumber", SqlDbType.NVarChar).Value = AssetNumber;

            SqlDataAdapter da = new SqlDataAdapter(request.Command);
            DataSet ds = new DataSet();
            da.Fill(ds);
            //create an asset and tag it onto sql_request
            request.Message = "success:getdata";
            request.Data = ds;
            request.Success = true;
            try
            {
                List<Asset> newassets = new List<Asset>();
                foreach (DataRow dr in request.Data.Tables[0].Rows)
                {
                    Asset a = new Asset();
                    a.AssetName = dr?.Field<string>("AssetName");
                    a.AssetNumber = dr?.Field<string>("AssetNumber");
                    try
                    {
                        a.OrderNumber = dr?.Field<string>("OrderNumber");
                    }
                    catch { }
                    a.ShipTo = dr?.Field<string>("ShipTo");

                    a.DateShipped = DateTime.Parse(dr?.Field<string>("DateShipped"));
                    a.ServiceEngineer = dr?.Field<string>("ServiceEngineer");
                    a.PersonShipping = dr?.Field<string>("PersonShipping");
                    a.DateRecieved = DateTime.Parse(dr?.Field<string>("DateReturned"));
                    a.weight = Convert.ToDecimal(dr?.Field<string>("AssetWeight"));
                    a.IsDamaged = dr.Field<bool>("Damaged");
                    a.OnHold = dr.Field<bool>("OnHold");
                    a.IsOut = dr.Field<bool>("IsOut");
                    a.Description = dr?.Field<string>("AssetDescription");
                    a.IsCalibrated = dr.Field<bool>("CalibratedAsset");
                    a.CalibrationCompany = dr?.Field<string>("CalibrationCompany");
                    a.LastCalibrated = DateTime.Parse(dr?.Field<string>("LastCalibrated"));
                    a.CalibrationPeriod = dr?.Field<string>("CalibrationPeriod");
                    var calXml = dr?.Field<string>("CalibrationHistory");
                    try
                    {
                        a.CalibrationHistory = new CalibrationLibrary().DeserializeFromXmlString<CalibrationLibrary>(calXml);
                    }
                    catch { }
                    a.Images = dr?.Field<string>("Images");
                    a.BarcodeImage = dr?.Field<string>("BarcodeImage");
                    try
                    {
                        var xml = dr?.Field<string>("History");
                        a.History = new AssetHistory().Deserialize(xml);
                        foreach (var ii in a.History.History)
                        {
                            ii.IsHistoryItem = true;
                        }
                    }
                    catch { }
                    a.PackingSlip = dr?.Field<string>("PackingSlip");
                    a.UpsLabel = dr?.Field<string>("UpsLabel");
                    a.ReturnReport = dr?.Field<string>("ReturnReport");
                    var doc_csv = dr?.Field<string>("Documents");
                    a.Documents = new List<string>();
                    if (doc_csv != null)
                    {
                        if (doc_csv != "")
                        {
                            a.Documents = doc_csv.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        }
                    }
                    request.Tag = a;
                }
                 
            }
            catch { }


        }
        catch (Exception ex)
        {
            request.Error.Ex = ex;
            request.Success = false;
            request.Message = "error:getdata";
            return request;
        }
        finally
        {
            if (close)
                CloseConnection(request);
        }
        return request;
    }
    public static SQL_Request DeleteAsset(this SQL_Request request, string AssetNumber, bool close = true)
    {
        request.Command = new SqlCommand();

        try
        {
            if (request.Connection.State == ConnectionState.Closed)
            { request.Connection.Open(); }
            // Create a object of SqlCommand class
            request.Command.Connection = request.Connection; //Pass the connection object to Command
            request.Command.CommandType = CommandType.StoredProcedure; // We will use stored procedure.

            request.Command.CommandText = "AssetDelete";
            request.Command.Parameters.Add("@AssetNumber", SqlDbType.NVarChar).Value = AssetNumber;
            request.Command.ExecuteNonQuery();
            request.Message = "success:deletedata";
           
            request.Success = true;

        }
        catch (Exception ex)
        {
            request.Error.Ex = ex;
            request.Success = false;
            request.Message = "error:deletedata";
            return request;
        }
        finally
        {
            if (close)
                CloseConnection(request);
        }
        return request;
    }
    public static SQL_Request UpdateAsset(this SQL_Request request, Asset asset, bool close = true)
    {
      
        request.Command = new SqlCommand();
        try
        {
            if (request.Connection.State == ConnectionState.Closed)
            { request.Connection.Open(); }
            // Create a object of SqlCommand class
            request.Command.Connection = request.Connection; //Pass the connection object to Command
            request.Command.CommandType = CommandType.StoredProcedure; // We will use stored procedure.
            request.Command.CommandText = "AssetUpdate";
            string imgs = asset.Images;            
            // request.Command.CommandText = "spUpdateTimeData"; //Stored Procedure Name
            request.Command.Parameters.Add("@AssetName", SqlDbType.NVarChar).Value = asset.AssetName;
            request.Command.Parameters.Add("@AssetNumber", SqlDbType.NVarChar).Value = asset.AssetNumber;
            request.Command.Parameters.Add("@CalibratedAsset", SqlDbType.Bit).Value = asset.IsCalibrated;
            request.Command.Parameters.Add("@Damaged", SqlDbType.Bit).Value = asset.IsDamaged;
            request.Command.Parameters.Add("@OnHold", SqlDbType.Bit).Value = asset.OnHold;
            request.Command.Parameters.Add("@IsOut", SqlDbType.Bit).Value = asset.IsOut;
            request.Command.Parameters.Add("@BarcodeImage", SqlDbType.NVarChar).Value = asset.BarcodeImage;
            request.Command.Parameters.Add("@CalibrationCompany", SqlDbType.NVarChar).Value = asset.CalibrationCompany;
            request.Command.Parameters.Add("@CalibrationPeriod", SqlDbType.NVarChar).Value = asset.CalibrationPeriod;
            request.Command.Parameters.Add("@DateReturned", SqlDbType.NVarChar).Value = asset.DateRecieved.ToString();
            request.Command.Parameters.Add("@DateShipped", SqlDbType.NVarChar).Value = asset.DateShipped.ToString();
            request.Command.Parameters.Add("@AssetDescription", SqlDbType.NVarChar).Value = asset.Description;
            request.Command.Parameters.Add("@LastCalibrated", SqlDbType.NVarChar).Value = asset.LastCalibrated.ToString();
            request.Command.Parameters.Add("@OrderNumber", SqlDbType.NVarChar).Value = asset.OrderNumber.ToString();
            request.Command.Parameters.Add("@PersonShipping", SqlDbType.NVarChar).Value = asset.PersonShipping;
            request.Command.Parameters.Add("@Images", SqlDbType.NVarChar).Value = imgs;
            request.Command.Parameters.Add("@ImageLinks", SqlDbType.NVarChar).Value = imgs;
            request.Command.Parameters.Add("@ServiceEngineer", SqlDbType.NVarChar).Value = asset.ServiceEngineer;
            request.Command.Parameters.Add("@ShipTo", SqlDbType.NVarChar).Value = asset.ShipTo;
            request.Command.Parameters.Add("@AssetWeight", SqlDbType.NVarChar).Value = asset.weight.ToString();
            if (asset.PackingSlip == null) { asset.PackingSlip = ""; }
            request.Command.Parameters.Add("@PackingSlip", SqlDbType.NVarChar).Value = asset.PackingSlip;
            request.Command.Parameters.Add("@ReturnReport", SqlDbType.NVarChar).Value = asset.ReturnReport;
            string doc_csv = "";
            if (asset.Documents!=null)
            {
                if (asset.Documents.Count>0)
                {
                    foreach (var item in asset.Documents)
                    {
                        doc_csv += item + ",";
                    }
                }
            }
            request.Command.Parameters.Add("@Documents", SqlDbType.NVarChar).Value = doc_csv;

            if (asset.UpsLabel==null)
            {
                asset.UpsLabel = "/Account/Templates/blank.pdf";
            }
            request.Command.Parameters.Add("@UPSlabel", SqlDbType.NVarChar).Value = asset.UpsLabel;

            string calXml = new CalibrationLibrary().SerializeToXmlString(new CalibrationLibrary());
            try
            {
                calXml = asset.CalibrationHistory.SerializeToXmlString(asset.CalibrationHistory);
            }
            catch { }
            request.Command.Parameters.Add("@CalibrationHistory", SqlDbType.NVarChar).Value = ""; ///ERASE CALIBRATION DATA SPACE
            try
            {
                foreach (var ii in asset.History.History)
                {
                    ii.IsHistoryItem = true;
                }
            }
            catch { }
            try
            {
                foreach(var aa in asset.History.History)
                {
                    aa.History = new AssetHistory();
                }
                var histxml = asset.History.Serialize();
                request.Command.Parameters.Add("@History", SqlDbType.NVarChar).Value = histxml;
            }
            catch { }

            request.Command.ExecuteNonQuery();
            request.Message = "success:assetUpdate";
        }
        catch (Exception ex)
        {
            request.Error.Ex = ex;
            request.Success = false;
            request.Message = "error:assetUpdate";

           // System.Windows.Forms.MessageBox.Show(ex.ToString());
            return request;
        }
        finally
        {
            if (close)
                CloseConnection(request);
        }
        return request;
    }
    //Asset Async
    public static async Task<SQL_Request> UpdateAssetAsync(this SQL_Request request, Asset asset, bool close = true)
    {

        request.Command = new SqlCommand();
        try
        {
            if (request.Connection.State == ConnectionState.Closed)
            { await request.Connection.OpenAsync(); }
            // Create a object of SqlCommand class
            request.Command.Connection = request.Connection; //Pass the connection object to Command
            request.Command.CommandType = CommandType.StoredProcedure; // We will use stored procedure.
            request.Command.CommandText = "AssetUpdate";
            string imgs = asset.Images;
            // request.Command.CommandText = "spUpdateTimeData"; //Stored Procedure Name
            request.Command.Parameters.Add("@AssetName", SqlDbType.NVarChar).Value = asset.AssetName;
            request.Command.Parameters.Add("@AssetNumber", SqlDbType.NVarChar).Value = asset.AssetNumber;
            request.Command.Parameters.Add("@CalibratedAsset", SqlDbType.Bit).Value = asset.IsCalibrated;
            request.Command.Parameters.Add("@Damaged", SqlDbType.Bit).Value = asset.IsDamaged;
            request.Command.Parameters.Add("@OnHold", SqlDbType.Bit).Value = asset.OnHold;
            request.Command.Parameters.Add("@IsOut", SqlDbType.Bit).Value = asset.IsOut;
            request.Command.Parameters.Add("@BarcodeImage", SqlDbType.NVarChar).Value = asset.BarcodeImage;
            request.Command.Parameters.Add("@CalibrationCompany", SqlDbType.NVarChar).Value = asset.CalibrationCompany;
            request.Command.Parameters.Add("@CalibrationPeriod", SqlDbType.NVarChar).Value = asset.CalibrationPeriod;
            request.Command.Parameters.Add("@DateReturned", SqlDbType.NVarChar).Value = asset.DateRecieved.ToString();
            request.Command.Parameters.Add("@DateShipped", SqlDbType.NVarChar).Value = asset.DateShipped.ToString();
            request.Command.Parameters.Add("@AssetDescription", SqlDbType.NVarChar).Value = asset.Description;
            request.Command.Parameters.Add("@LastCalibrated", SqlDbType.NVarChar).Value = asset.LastCalibrated.ToString();
            request.Command.Parameters.Add("@OrderNumber", SqlDbType.NVarChar).Value = asset.OrderNumber.ToString();
            request.Command.Parameters.Add("@PersonShipping", SqlDbType.NVarChar).Value = asset.PersonShipping;
            request.Command.Parameters.Add("@Images", SqlDbType.NVarChar).Value = imgs;
            request.Command.Parameters.Add("@ImageLinks", SqlDbType.NVarChar).Value = imgs;
            request.Command.Parameters.Add("@ServiceEngineer", SqlDbType.NVarChar).Value = asset.ServiceEngineer;
            request.Command.Parameters.Add("@ShipTo", SqlDbType.NVarChar).Value = asset.ShipTo;
            request.Command.Parameters.Add("@AssetWeight", SqlDbType.NVarChar).Value = asset.weight.ToString();
            if (asset.PackingSlip == null) { asset.PackingSlip = ""; }
            request.Command.Parameters.Add("@PackingSlip", SqlDbType.NVarChar).Value = asset.PackingSlip;
            request.Command.Parameters.Add("@ReturnReport", SqlDbType.NVarChar).Value = asset.ReturnReport;
            request.Command.Parameters.Add("@UPSlabel", SqlDbType.NVarChar).Value = asset.UpsLabel;
            string doc_csv = "";
            if (asset.Documents != null)
            {
                if (asset.Documents.Count > 0)
                {
                    foreach (var item in asset.Documents)
                    {
                        doc_csv += item + ",";
                    }
                }
            }
            request.Command.Parameters.Add("@Documents", SqlDbType.NVarChar).Value = doc_csv;
            string calXml = new CalibrationLibrary().SerializeToXmlString(new CalibrationLibrary());
            try
            {
                calXml = asset.CalibrationHistory.SerializeToXmlString(asset.CalibrationHistory);
            }
            catch { }
            request.Command.Parameters.Add("@CalibrationHistory", SqlDbType.NVarChar).Value = calXml;
            try
            {
                foreach (var ii in asset.History.History)
                {
                    ii.IsHistoryItem = true;
                }
            }
            catch { }
            try
            {
                foreach (var aa in asset.History.History)
                {
                    aa.History = new AssetHistory();
                }
                var histxml = asset.History.Serialize();
                request.Command.Parameters.Add("@History", SqlDbType.NVarChar).Value = histxml;
            }
            catch { }

           await request.Command.ExecuteNonQueryAsync();
            request.Message = "success:assetUpdate";
        }
        catch (Exception ex)
        {
            request.Error.Ex = ex;
            request.Success = false;
            request.Message = "error:assetUpdate";

            //System.Windows.Forms.MessageBox.Show(ex.ToString());
            return request;
        }
        finally
        {
            if (close)
                CloseConnection(request);
        }
        return request;
    }
    public static async Task<SQL_Request> GetAssetAsync(this SQL_Request request, string AssetNumber, bool close = true)
    {
        request.Command = new SqlCommand();

        try
        {
            if (request.Connection.State == ConnectionState.Closed)
            { await request.Connection.OpenAsync(); }
            // Create a object of SqlCommand class
            request.Command.Connection = request.Connection; //Pass the connection object to Command
            request.Command.CommandType = CommandType.StoredProcedure; // We will use stored procedure.

            request.Command.CommandText = "AssetGet";
            //request.Command.CommandText = "spGetTimeData"; //Stored Procedure Name

            request.Command.Parameters.Add("@AssetNumber", SqlDbType.NVarChar).Value = AssetNumber;

            SqlDataAdapter da = new SqlDataAdapter(request.Command);
            DataSet ds = new DataSet();
            try
            {
                await Task.Run(() => da.Fill(ds));
            }
            catch (Exception ex)
            {
                request.Error.Ex = ex;
                request.Success = false;
                request.Message = "error:getalldata";
                return request;
            }
            //create an asset and tag it onto sql_request
            request.Message = "success:getdata";
            request.Data = ds;
            request.Success = true;
            try
            {
                List<Asset> newassets = new List<Asset>();
                foreach (DataRow dr in request.Data.Tables[0].Rows)
                {
                    Asset a = new Asset();
                    a.AssetName = dr?.Field<string>("AssetName");
                    a.AssetNumber = dr?.Field<string>("AssetNumber");
                    try
                    {
                        a.OrderNumber = dr?.Field<string>("OrderNumber");
                    }
                    catch { }
                    a.ShipTo = dr?.Field<string>("ShipTo");

                    a.DateShipped = DateTime.Parse(dr?.Field<string>("DateShipped"));
                    a.ServiceEngineer = dr?.Field<string>("ServiceEngineer");
                    a.PersonShipping = dr?.Field<string>("PersonShipping");
                    a.DateRecieved = DateTime.Parse(dr?.Field<string>("DateReturned"));
                    a.weight = Convert.ToDecimal(dr?.Field<string>("AssetWeight"));
                    a.IsDamaged = dr.Field<bool>("Damaged");
                    a.OnHold = dr.Field<bool>("OnHold");
                    a.IsOut = dr.Field<bool>("IsOut");
                    a.Description = dr?.Field<string>("AssetDescription");
                    a.IsCalibrated = dr.Field<bool>("CalibratedAsset");
                    a.CalibrationCompany = dr?.Field<string>("CalibrationCompany");
                    a.LastCalibrated = DateTime.Parse(dr?.Field<string>("LastCalibrated"));
                    a.CalibrationPeriod = dr?.Field<string>("CalibrationPeriod");
                    var calXml = dr?.Field<string>("CalibrationHistory");
                    try
                    {
                        a.CalibrationHistory = new CalibrationLibrary().DeserializeFromXmlString<CalibrationLibrary>(calXml);
                    }
                    catch { }
                    a.Images = dr?.Field<string>("Images");
                    a.BarcodeImage = dr?.Field<string>("BarcodeImage");
                    try
                    {
                        var xml = dr?.Field<string>("History");
                        a.History = new AssetHistory().Deserialize(xml);
                        foreach (var ii in a.History.History)
                        {
                            ii.IsHistoryItem = true;
                        }
                    }
                    catch { }
                    a.PackingSlip = dr?.Field<string>("PackingSlip");
                    a.UpsLabel = dr?.Field<string>("UpsLabel");
                    a.ReturnReport = dr?.Field<string>("ReturnReport");
                    var doc_csv = dr?.Field<string>("Documents");
                    a.Documents = new List<string>();
                    if (doc_csv != null)
                    {
                        if (doc_csv != "")
                        {
                            a.Documents = doc_csv.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        }
                    }
                    request.Tag = a;
                }

            }
            catch { }


        }
        catch (Exception ex)
        {
            request.Error.Ex = ex;
            request.Success = false;
            request.Message = "error:getdata";
            return request;
        }
        finally
        {
            if (close)
                CloseConnection(request);
        }
        return request;
    }
    public static async Task<SQL_Request> GetAssetsAsync(this SQL_Request request, bool close = true)
    {
        request.Command = new SqlCommand();
        try
        {
            if (request.Connection.State == ConnectionState.Closed)
            { await request.Connection.OpenAsync(); }
            request.Command.Connection = request.Connection; //Pass the connection object to Command
            request.Command.CommandType = CommandType.StoredProcedure; // We will use stored procedure.
            request.Command.CommandText = "AssetGetAll";
            request.Command.CommandTimeout = 120;
            SqlDataAdapter da = new SqlDataAdapter(request.Command);
            DataSet ds = new DataSet();
            try
            {
                await Task.Run(() => da.Fill(ds));
            }
            catch (Exception ex)
            {
                request.Error.Ex = ex;
                request.Success = false;
                request.Message = "error:getalldata";
                return request;
            }
            request.Message = "success:getalldata";
            request.Data = ds;
            request.Success = true;
            List<Asset> newassets = new List<Asset>();
            try
            {

                foreach (DataRow dr in request.Data.Tables[0].Rows)
                {
                    Asset a = new Asset();
                    a.AssetName = dr?.Field<string>("AssetName");
                    a.AssetNumber = dr?.Field<string>("AssetNumber");
                    try
                    {
                        a.OrderNumber = dr?.Field<string>("OrderNumber");
                    }
                    catch { }
                    a.ShipTo = dr?.Field<string>("ShipTo");
                    a.IsOut = dr.Field<bool>("IsOut");
                    a.DateShipped = DateTime.Parse(dr?.Field<string>("DateShipped"));
                    a.ServiceEngineer = dr?.Field<string>("ServiceEngineer");
                    a.PersonShipping = dr?.Field<string>("PersonShipping");
                    a.DateRecieved = DateTime.Parse(dr?.Field<string>("DateReturned"));
                    a.weight = Convert.ToDecimal(dr?.Field<string>("AssetWeight"));
                    a.IsDamaged = dr.Field<bool>("Damaged");
                    a.OnHold = dr.Field<bool>("OnHold");
                    a.Description = dr?.Field<string>("AssetDescription");
                    a.IsCalibrated = dr.Field<bool>("CalibratedAsset");
                    a.CalibrationCompany = dr?.Field<string>("CalibrationCompany");
                    a.LastCalibrated = DateTime.Parse(dr?.Field<string>("LastCalibrated"));
                    a.CalibrationPeriod = dr?.Field<string>("CalibrationPeriod");
                    var calXml = dr?.Field<string>("CalibrationHistory");
                    try
                    {
                        a.CalibrationHistory = new CalibrationLibrary().DeserializeFromXmlString<CalibrationLibrary>(calXml);
                    }
                    catch { }

                    a.Images = dr?.Field<string>("Images");
                    a.BarcodeImage = dr?.Field<string>("BarcodeImage");
                    try
                    {
                        var xml = dr?.Field<string>("History");
                        a.History = new AssetHistory().Deserialize(xml);
                        foreach (var ii in a.History.History)
                        {
                            ii.IsHistoryItem = true;
                        }
                    }
                    catch { }
                    a.PackingSlip = dr?.Field<string>("PackingSlip");
                    a.UpsLabel = dr?.Field<string>("UpsLabel");
                    a.ReturnReport = dr?.Field<string>("ReturnReport");
                    var doc_csv = dr?.Field<string>("Documents");
                    a.Documents = new List<string>();
                    if (doc_csv != null)
                    {
                        if (doc_csv != "")
                        {
                            a.Documents = doc_csv.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        }
                    }
                    newassets.Add(a);
                }
                request.Tag = newassets;
            }
            catch { }

        }
        catch (Exception ex)
        {
            request.Error.Ex = ex;
            request.Success = false;
            request.Message = "error:getalldata";
            return request;
        }
        finally
        {
            if (close)
            {
                CloseConnection(request);
            }
        }
        return request;
    }
    public static async Task<SQL_Request> AddAssetAsync(this SQL_Request request, Asset asset, bool close = true)
    {
        if (request.Connection.State == ConnectionState.Closed)
        { await request.Connection.OpenAsync(); }
        request.Command = new SqlCommand();

        try
        {
            string imgs = "";

            imgs = asset.Images;
            if (request.Connection.State == ConnectionState.Closed)
            { request.Connection.Open(); }
            request.Command = new SqlCommand();
            request.Command.Connection = request.Connection; //Pass the connection object to Command
            request.Command.CommandType = CommandType.StoredProcedure; // We will use stored procedure.
            request.Command.CommandText = "AssetInsert"; //Stored Procedure Name

            //	 @AssetName,@AssetNumber,@CalibratedAsset,@Damaged,@OnHold,@BarcodeImage,
            //@CalibrationCompany,@CalibrationHistory,@CalibrationPeriod,@DateReturned,
            //@DateShipped,@AssetDescription,@LastCalibrated,@OrderNumber,@PersonShipping,
            //@PackingSlip,@ReturnReport,@UPSlabel,@Images,@ImageLinks,@ServiceEngineer,@ShipTo,@AssetWeight
            request.Command.Parameters.Add("@AssetName", SqlDbType.NVarChar).Value = asset.AssetName;
            request.Command.Parameters.Add("@AssetNumber", SqlDbType.NVarChar).Value = asset.AssetNumber;
            request.Command.Parameters.Add("@CalibratedAsset", SqlDbType.Bit).Value = asset.IsCalibrated;
            request.Command.Parameters.Add("@Damaged", SqlDbType.Bit).Value = asset.IsDamaged;
            request.Command.Parameters.Add("@OnHold", SqlDbType.Bit).Value = asset.OnHold;
            request.Command.Parameters.Add("@IsOut", SqlDbType.Bit).Value = asset.IsOut;
            request.Command.Parameters.Add("@BarcodeImage", SqlDbType.NVarChar).Value = asset.BarcodeImage; //FIX
            request.Command.Parameters.Add("@CalibrationCompany", SqlDbType.NVarChar).Value = asset.CalibrationCompany;
            string calXml = new CalibrationLibrary().SerializeToXmlString(new CalibrationLibrary());
            try
            {
                calXml = asset.CalibrationHistory.SerializeToXmlString(asset.CalibrationHistory);
            }
            catch { }
            request.Command.Parameters.Add("@CalibrationHistory", SqlDbType.NVarChar).Value = calXml;
            request.Command.Parameters.Add("@CalibrationPeriod", SqlDbType.NVarChar).Value = asset.CalibrationPeriod;
            request.Command.Parameters.Add("@DateReturned", SqlDbType.NVarChar).Value = asset.DateRecieved.ToString();
            request.Command.Parameters.Add("@DateShipped", SqlDbType.NVarChar).Value = asset.DateShipped.ToString();
            request.Command.Parameters.Add("@AssetDescription", SqlDbType.NVarChar).Value = asset.Description;
            request.Command.Parameters.Add("@LastCalibrated", SqlDbType.NVarChar).Value = asset.LastCalibrated.ToString();
            request.Command.Parameters.Add("@OrderNumber", SqlDbType.NVarChar).Value = asset.OrderNumber.ToString();
            request.Command.Parameters.Add("@PersonShipping", SqlDbType.NVarChar).Value = asset.PersonShipping;
            request.Command.Parameters.Add("@Images", SqlDbType.NVarChar).Value = imgs;
            request.Command.Parameters.Add("@ImageLinks", SqlDbType.NVarChar).Value = imgs;
            request.Command.Parameters.Add("@ServiceEngineer", SqlDbType.NVarChar).Value = asset.ServiceEngineer;
            request.Command.Parameters.Add("@ShipTo", SqlDbType.NVarChar).Value = asset.ShipTo;
            request.Command.Parameters.Add("@AssetWeight", SqlDbType.NVarChar).Value = asset.weight.ToString();
            try
            {
                foreach (var ii in asset.History.History)
                {
                    ii.IsHistoryItem = true;
                }
            }
            catch { }
            var histxml = asset.History.Serialize();
            request.Command.Parameters.Add("@History", SqlDbType.NVarChar).Value = histxml;
            request.Command.Parameters.Add("@PackingSlip", SqlDbType.NVarChar).Value = asset.PackingSlip;
            request.Command.Parameters.Add("@ReturnReport", SqlDbType.NVarChar).Value = asset.ReturnReport;
            request.Command.Parameters.Add("@UPSlabel", SqlDbType.NVarChar).Value = asset.UpsLabel;
            string doc_csv = "";
            if (asset.Documents != null)
            {
                if (asset.Documents.Count > 0)
                {
                    foreach (var item in asset.Documents)
                    {
                        doc_csv += item + ",";
                    }
                }
            }
            request.Command.Parameters.Add("@Documents", SqlDbType.NVarChar).Value = doc_csv;
            await request.Command.ExecuteNonQueryAsync();
            request.Success = true;
            request.Message = "success:addAsset";

        }
        catch (Exception ex)
        {
            request.Error.Ex = ex;
            request.Success = false;
            request.Message = "error:addAsset";
            return request;
        }
        finally
        {
            if (close)
                CloseConnection(request);

        }
        request.Message = "success:addAsset";
        request.Success = true;
        return request;
    }

    //Settings   AWP_STARRAG_US
    public static SQL_Request SettingsAdd(this SQL_Request request, string AppName, string XmlData, string XmlData2 = "", string XmlData3 = "", string XmlData4 = "", string XmlData5="", bool close = true)
    {
        if (request.Connection.State == ConnectionState.Closed)
        { request.Connection.Open(); }
        request.Command = new SqlCommand();

        try
        {          
            if (request.Connection.State == ConnectionState.Closed)
            { request.Connection.Open(); }
            request.Command = new SqlCommand();
            request.Command.Connection = request.Connection; //Pass the connection object to Command
            request.Command.CommandType = CommandType.StoredProcedure; // We will use stored procedure.
            request.Command.CommandText = "SettingsInsert"; //Stored Procedure Name
            
            request.Command.Parameters.Add("@AppName", SqlDbType.NVarChar).Value =AppName;
            request.Command.Parameters.Add("@XmlData", SqlDbType.NVarChar).Value =XmlData;
            request.Command.Parameters.Add("@XmlData2", SqlDbType.NVarChar).Value = XmlData2;
            request.Command.Parameters.Add("@XmlData3", SqlDbType.NVarChar).Value = XmlData3;
            request.Command.Parameters.Add("@XmlData4", SqlDbType.NVarChar).Value = XmlData4;
            request.Command.Parameters.Add("@XmlData5", SqlDbType.NVarChar).Value = XmlData5;



            request.Command.ExecuteNonQuery();
            request.Success = true;
            request.Message = "success";

        }
        catch (Exception ex)
        {
            request.Error.Ex = ex;
            request.Success = false;
            request.Message = "error";
            return request;
        }
        finally
        {
            if (close)
                CloseConnection(request);

        }
        request.Message = "success";
        request.Success = true;
        return request;
    }
    public static SQL_Request SettingsGetAll(this SQL_Request request, bool close = true)
    {
        request.Command = new SqlCommand();

        try
        {
            if (request.Connection.State == ConnectionState.Closed)
            { request.Connection.Open(); }
            // Create a object of SqlCommand class
            request.Command.Connection = request.Connection; //Pass the connection object to Command
            request.Command.CommandType = CommandType.StoredProcedure; // We will use stored procedure.

            request.Command.CommandText = "SettingsGetAll";
           
            SqlDataAdapter da = new SqlDataAdapter(request.Command);
            DataSet ds = new DataSet();
            da.Fill(ds);
            request.Message = "success:getalldata";
            request.Data = ds;
            request.Success = true;
            List<SettingsDBData> settingslist = new List<SettingsDBData>();
            try
            {
                foreach (DataRow dr in request.Data.Tables[0].Rows)
                {
                    try
                    {
                        SettingsDBData a = new SettingsDBData();
                        a.Appname = dr?.Field<string>("AppName");
                        a.XmlData = dr?.Field<string>("XmlData");
                        a.XmlData2 = dr?.Field<string>("XmlData2");
                        a.XmlData3 = dr?.Field<string>("XmlData3");
                        a.XmlData4 = dr?.Field<string>("XmlData4");
                        a.XmlData5 = dr?.Field<string>("XmlData5");

                        settingslist.Add(a);
                    }
                    catch { }
                }
                request.Tag = settingslist;
            }
            catch { }

        }
        catch (Exception ex)
        {
            request.Error.Ex = ex;
            request.Success = false;
            request.Message = "error:getalldata";
            return request;
        }
        finally
        {
            if (close)
                CloseConnection(request);
        }
        return request;
    }
    public static SQL_Request SettingsGet(this SQL_Request request, string AppName= "AWP_STARRAG_US", bool close = true)
    {
        request.Command = new SqlCommand();

        try
        {
            if (request.Connection.State == ConnectionState.Closed)
            { request.Connection.Open(); }
            // Create a object of SqlCommand class
            request.Command.Connection = request.Connection; //Pass the connection object to Command
            request.Command.CommandType = CommandType.StoredProcedure; // We will use stored procedure.

            request.Command.CommandText = "SettingsGet";
            //request.Command.CommandText = "spGetTimeData"; //Stored Procedure Name

            request.Command.Parameters.Add("@AppName", SqlDbType.NVarChar).Value = AppName;

            SqlDataAdapter da = new SqlDataAdapter(request.Command);
            DataSet ds = new DataSet();
            da.Fill(ds);
            //create an asset and tag it onto sql_request
            request.Message = "success:getdata";
            request.Data = ds;
            request.Success = true;
            try
            {
                List<SettingsDBData> newassets = new List<SettingsDBData>();
                foreach (DataRow dr in request.Data.Tables[0].Rows)
                {
                    SettingsDBData a = new SettingsDBData();
                    a.Appname = dr?.Field<string>("AppName");
                    a.XmlData = dr?.Field<string>("XmlData");
                    a.XmlData2 = dr?.Field<string>("XmlData2");
                    a.XmlData3 = dr?.Field<string>("XmlData3");
                    a.XmlData4 = dr?.Field<string>("XmlData4");
                    a.XmlData5 = dr?.Field<string>("XmlData5");
                    request.Tag = newassets;
                    newassets.Add(a);
                }
                if (newassets.Count==1)
                {
                    request.Tag = newassets.FirstOrDefault();
                }
                

            }
            catch { }


        }
        catch (Exception ex)
        {
            request.Error.Ex = ex;
            request.Success = false;
            request.Message = "error:getdata";
            return request;
        }
        finally
        {
            if (close)
                CloseConnection(request);
        }
        return request;
    }
    public static SQL_Request SettingsDelete(this SQL_Request request, string AppName= "AWP_STARRAG_US", bool close = true)
    {
        request.Command = new SqlCommand();

        try
        {
            if (request.Connection.State == ConnectionState.Closed)
            { request.Connection.Open(); }
            // Create a object of SqlCommand class
            request.Command.Connection = request.Connection; //Pass the connection object to Command
            request.Command.CommandType = CommandType.StoredProcedure; // We will use stored procedure.

            request.Command.CommandText = "SettingsDelete";
            request.Command.Parameters.Add("@AppName", SqlDbType.NVarChar).Value = AppName;
            request.Command.ExecuteNonQuery();
            request.Message = "success:deletedata";

            request.Success = true;

        }
        catch (Exception ex)
        {
            request.Error.Ex = ex;
            request.Success = false;
            request.Message = "error:deletedata";
            return request;
        }
        finally
        {
            if (close)
                CloseConnection(request);
        }
        return request;
    }
    public static SQL_Request SettingsUpdate(this SQL_Request request, string AppName, string XmlData, string XmlData2 = "", string XmlData3 = "", string XmlData4 = "", string XmlData5 = "", bool close = true)
    {

        request.Command = new SqlCommand();
        try
        {
            if (request.Connection.State == ConnectionState.Closed)
            { request.Connection.Open(); }
            // Create a object of SqlCommand class
            request.Command.Connection = request.Connection; //Pass the connection object to Command
            request.Command.CommandType = CommandType.StoredProcedure; // We will use stored procedure.
            request.Command.CommandText = "SettingsUpdate";
            // request.Command.CommandText = "spUpdateTimeData"; //Stored Procedure Name

            request.Command.Parameters.Add("@AppName", SqlDbType.NVarChar).Value = AppName;
            request.Command.Parameters.Add("@XmlData", SqlDbType.NVarChar).Value = XmlData;
            request.Command.Parameters.Add("@XmlData2", SqlDbType.NVarChar).Value = XmlData2;
            request.Command.Parameters.Add("@XmlData3", SqlDbType.NVarChar).Value = XmlData3;
            request.Command.Parameters.Add("@XmlData4", SqlDbType.NVarChar).Value = XmlData4;
            request.Command.Parameters.Add("@XmlData5", SqlDbType.NVarChar).Value = XmlData5;

            request.Command.ExecuteNonQuery();
            request.Message = "success:assetUpdate";
        }
        catch (Exception ex)
        {
            request.Error.Ex = ex;
            request.Success = false;
            request.Message = "error:assetUpdate";
            return request;
        }
        finally
        {
            if (close)
                CloseConnection(request);
        }
        return request;
    }
    //Settings Async
    public static async Task<SQL_Request> SettingsAddAsync(this SQL_Request request, string AppName, string XmlData, string XmlData2 = "", string XmlData3 = "", string XmlData4 = "", string XmlData5 = "", bool close = true)
    {
        if (request.Connection.State == ConnectionState.Closed)
        { await request.Connection.OpenAsync(); }
        request.Command = new SqlCommand();

        try
        {
            if (request.Connection.State == ConnectionState.Closed)
            { request.Connection.Open(); }
            request.Command = new SqlCommand();
            request.Command.Connection = request.Connection; //Pass the connection object to Command
            request.Command.CommandType = CommandType.StoredProcedure; // We will use stored procedure.
            request.Command.CommandText = "SettingsInsert"; //Stored Procedure Name

            request.Command.Parameters.Add("@AppName", SqlDbType.NVarChar).Value = AppName;
            request.Command.Parameters.Add("@XmlData", SqlDbType.NVarChar).Value = XmlData;
            request.Command.Parameters.Add("@XmlData2", SqlDbType.NVarChar).Value = XmlData2;
            request.Command.Parameters.Add("@XmlData3", SqlDbType.NVarChar).Value = XmlData3;
            request.Command.Parameters.Add("@XmlData4", SqlDbType.NVarChar).Value = XmlData4;
            request.Command.Parameters.Add("@XmlData5", SqlDbType.NVarChar).Value = XmlData5;



            await request.Command.ExecuteNonQueryAsync();
            request.Success = true;
            request.Message = "success";

        }
        catch (Exception ex)
        {
            request.Error.Ex = ex;
            request.Success = false;
            request.Message = "error";
            return request;
        }
        finally
        {
            if (close)
                CloseConnection(request);
                
        }
        request.Message = "success";
        request.Success = true;
        return request;
    }
    public static async Task<SQL_Request> SettingsGetAllAsync(this SQL_Request request, bool close = true)
    {
        request.Command = new SqlCommand();

        try
        {
            if (request.Connection.State == ConnectionState.Closed)
            { await request.Connection.OpenAsync(); }
            // Create a object of SqlCommand class
            request.Command.Connection = request.Connection; //Pass the connection object to Command
            request.Command.CommandType = CommandType.StoredProcedure; // We will use stored procedure.

            request.Command.CommandText = "SettingsGetAll";

            SqlDataAdapter da = new SqlDataAdapter(request.Command);
            DataSet ds = new DataSet();
            try
            {
                await Task.Run(() => da.Fill(ds));
            }
            catch (Exception ex)
            {
                request.Error.Ex = ex;
                request.Success = false;
                request.Message = ex.ToString();
                return request;
            }
            request.Message = "success:getalldata";
            request.Data = ds;
            request.Success = true;
            List<SettingsDBData> settingslist = new List<SettingsDBData>();
            try
            {
                foreach (DataRow dr in request.Data.Tables[0].Rows)
                {
                    try
                    {
                        SettingsDBData a = new SettingsDBData();
                        a.Appname = dr?.Field<string>("AppName");
                        a.XmlData = dr?.Field<string>("XmlData");
                        a.XmlData2 = dr?.Field<string>("XmlData2");
                        a.XmlData3 = dr?.Field<string>("XmlData3");
                        a.XmlData4 = dr?.Field<string>("XmlData4");
                        a.XmlData5 = dr?.Field<string>("XmlData5");

                        settingslist.Add(a);
                    }
                    catch { }
                }
                request.Tag = settingslist;
            }
            catch { }

        }
        catch (Exception ex)
        {
            request.Error.Ex = ex;
            request.Success = false;
            request.Message = "error:getalldata";
            return request;
        }
        finally
        {
            if (close)
                CloseConnection(request);
        }
        return request;
    }
    public static async Task<SQL_Request> SettingsGetAsync(this SQL_Request request, string AppName= "AWP_STARRAG_US", bool close = true)
    {
        request.Command = new SqlCommand();

        try
        {
            if (request.Connection.State == ConnectionState.Closed)
            { await request.Connection.OpenAsync(); }
            // Create a object of SqlCommand class
            request.Command.Connection = request.Connection; //Pass the connection object to Command
            request.Command.CommandType = CommandType.StoredProcedure; // We will use stored procedure.

            request.Command.CommandText = "SettingsGet";
            //request.Command.CommandText = "spGetTimeData"; //Stored Procedure Name

            request.Command.Parameters.Add("@AppName", SqlDbType.NVarChar).Value = AppName;

            SqlDataAdapter da = new SqlDataAdapter(request.Command);
            DataSet ds = new DataSet();
            try
            {
                await Task.Run(() => da.Fill(ds));
            }
            catch (Exception ex)
            {
                request.Error.Ex = ex;
                request.Success = false;
                request.Message = "error:getdata";
                return request;
            }
            //create an asset and tag it onto sql_request
            request.Message = "success:getdata";
            request.Data = ds;
            request.Success = true;
            try
            {
                List<SettingsDBData> newassets = new List<SettingsDBData>();
                foreach (DataRow dr in request.Data.Tables[0].Rows)
                {
                    SettingsDBData a = new SettingsDBData();
                    a.Appname = dr?.Field<string>("AppName");
                    a.XmlData = dr?.Field<string>("XmlData");
                    a.XmlData2 = dr?.Field<string>("XmlData2");
                    a.XmlData3 = dr?.Field<string>("XmlData3");
                    a.XmlData4 = dr?.Field<string>("XmlData4");
                    a.XmlData5 = dr?.Field<string>("XmlData5");
                    request.Tag = newassets;
                    newassets.Add(a);
                }
                if (newassets.Count==1)
                {
                    request.Tag = newassets.FirstOrDefault();
                }
                

            }
            catch { }


        }
        catch (Exception ex)
        {
            request.Error.Ex = ex;
            request.Success = false;
            request.Message = "error:getdata";
            return request;
        }
        finally
        {
            if (close)
                CloseConnection(request);
        }
        return request;
    }
    public static async Task<SQL_Request> SettingsUpdateAsync(this SQL_Request request, string AppName, string XmlData, string XmlData2 = "", string XmlData3 = "", string XmlData4 = "", string XmlData5 = "", bool close = true)
    {

        request.Command = new SqlCommand();
        try
        {
            if (request.Connection.State == ConnectionState.Closed)
            { await request.Connection.OpenAsync(); }
            // Create a object of SqlCommand class
            request.Command.Connection = request.Connection; //Pass the connection object to Command
            request.Command.CommandType = CommandType.StoredProcedure; // We will use stored procedure.
            request.Command.CommandText = "SettingsUpdate";
            // request.Command.CommandText = "spUpdateTimeData"; //Stored Procedure Name

            request.Command.Parameters.Add("@AppName", SqlDbType.NVarChar).Value = AppName;
            request.Command.Parameters.Add("@XmlData", SqlDbType.NVarChar).Value = XmlData;
            request.Command.Parameters.Add("@XmlData2", SqlDbType.NVarChar).Value = XmlData2;
            request.Command.Parameters.Add("@XmlData3", SqlDbType.NVarChar).Value = XmlData3;
            request.Command.Parameters.Add("@XmlData4", SqlDbType.NVarChar).Value = XmlData4;
            request.Command.Parameters.Add("@XmlData5", SqlDbType.NVarChar).Value = XmlData5;

            await request.Command.ExecuteNonQueryAsync();
            request.Message = "success:assetUpdate";
        }
        catch (Exception ex)
        {
            request.Error.Ex = ex;
            request.Success = false;
            request.Message = "error:assetUpdate";
            return request;
        }
        finally
        {
            if (close)
                CloseConnection(request);
        }
        return request;
    }

    //Transactions
    public static SQL_Request TransactionsGetAll(this SQL_Request request, string AppName= "AWP_STARRAG_US", bool close = true)
    {
        request.Command = new SqlCommand();

        try
        {
            if (request.Connection.State == ConnectionState.Closed)
            { request.Connection.Open(); }
            // Create a object of SqlCommand class
            request.Command.Connection = request.Connection; //Pass the connection object to Command
            request.Command.CommandType = CommandType.StoredProcedure; // We will use stored procedure.

            request.Command.CommandText = "TransactionGetAll";
            request.Command.Parameters.Add("@AppName", SqlDbType.NVarChar).Value = AppName;

            SqlDataAdapter da = new SqlDataAdapter(request.Command);
            DataSet ds = new DataSet();
            da.Fill(ds);
            request.Message = "success:getalldata";
            request.Data = ds;
            request.Success = true;
            List<TransationDBData> settingslist = new List<TransationDBData>();
            try
            {
                foreach (DataRow dr in request.Data.Tables[0].Rows)
                {
                    try
                    {
                        TransationDBData a = new TransationDBData();
                        a.Appname = dr?.Field<string>("AppName");
                        a.XmlData = dr?.Field<string>("XmlData");
                        a.TransactionID = dr?.Field<string>("TransactionID");
                        settingslist.Add(a);
                    }
                    catch { }
                }
                request.Tag = settingslist;
            }
            catch { }

        }
        catch (Exception ex)
        {
            request.Error.Ex = ex;
            request.Success = false;
            request.Message = "error:getalldata";
            return request;
        }
        finally
        {
            if (close)
                CloseConnection(request);
        }
        return request;
    }
    public static SQL_Request TransactionAdd(this SQL_Request request, string AppName, string XmlData, string transactionID, bool close = true)
    {
        if (request.Connection.State == ConnectionState.Closed)
        { request.Connection.Open(); }
        request.Command = new SqlCommand();

        try
        {
            if (request.Connection.State == ConnectionState.Closed)
            { request.Connection.Open(); }
            request.Command = new SqlCommand();
            request.Command.Connection = request.Connection; //Pass the connection object to Command
            request.Command.CommandType = CommandType.StoredProcedure; // We will use stored procedure.
            request.Command.CommandText = "TransactionInsert"; //Stored Procedure Name

            request.Command.Parameters.Add("@AppName", SqlDbType.NVarChar).Value = AppName;
            request.Command.Parameters.Add("@XmlData", SqlDbType.NVarChar).Value = XmlData;
            request.Command.Parameters.Add("@TransactionID", SqlDbType.NVarChar).Value = transactionID;    
            request.Command.ExecuteNonQuery();
            request.Success = true;
            request.Message = "success";

        }
        catch (Exception ex)
        {
            request.Error.Ex = ex;
            request.Success = false;
            request.Message = "error";
            return request;
        }
        finally
        {
            if (close)
                CloseConnection(request);

        }
        request.Message = "success";
        request.Success = true;
        return request;
    }
    public static SQL_Request TransactionGet(this SQL_Request request, string transactionID, string AppName = "AWP_STARRAG_US", bool close = true)
    {
        request.Command = new SqlCommand();

        try
        {
            if (request.Connection.State == ConnectionState.Closed)
            { request.Connection.Open(); }
            // Create a object of SqlCommand class
            request.Command.Connection = request.Connection; //Pass the connection object to Command
            request.Command.CommandType = CommandType.StoredProcedure; // We will use stored procedure.

            request.Command.CommandText = "TransactionGet";
            //request.Command.CommandText = "spGetTimeData"; //Stored Procedure Name

            request.Command.Parameters.Add("@AppName", SqlDbType.NVarChar).Value = AppName;
            request.Command.Parameters.Add("@TransactionID", SqlDbType.NVarChar).Value = transactionID;

            SqlDataAdapter da = new SqlDataAdapter(request.Command);
            DataSet ds = new DataSet();
            da.Fill(ds);
            //create an asset and tag it onto sql_request
            request.Message = "success:getdata";
            request.Data = ds;
            request.Success = true;
            try
            {
                List<TransationDBData> newassets = new List<TransationDBData>();
                foreach (DataRow dr in request.Data.Tables[0].Rows)
                {
                    TransationDBData a = new TransationDBData();
                    a.Appname = dr?.Field<string>("AppName");
                    a.XmlData = dr?.Field<string>("XmlData");
                    a.TransactionID = dr?.Field<string>("TransactionID");                    
                    newassets.Add(a);
                    request.Tag = newassets;
                }
                if (newassets.Count == 1)
                {
                    request.Tag = newassets.FirstOrDefault();
                }


            }
            catch { }


        }
        catch (Exception ex)
        {
            request.Error.Ex = ex;
            request.Success = false;
            request.Message = "error:getdata";
            return request;
        }
        finally
        {
            if (close)
                CloseConnection(request);
        }
        return request;
    }
    public static SQL_Request TransactionDelete(this SQL_Request request,string transactionID, string AppName = "AWP_STARRAG_US", bool close = true)
    {
        request.Command = new SqlCommand();

        try
        {
            if (request.Connection.State == ConnectionState.Closed)
            { request.Connection.Open(); }
            // Create a object of SqlCommand class
            request.Command.Connection = request.Connection; //Pass the connection object to Command
            request.Command.CommandType = CommandType.StoredProcedure; // We will use stored procedure.

            request.Command.CommandText = "TransactionDelete";
            request.Command.Parameters.Add("@AppName", SqlDbType.NVarChar).Value = AppName;
            request.Command.Parameters.Add("@TransactionID", SqlDbType.NVarChar).Value = transactionID;

            request.Command.ExecuteNonQuery();
            request.Message = "success:deletedata";

            request.Success = true;

        }
        catch (Exception ex)
        {
            request.Error.Ex = ex;
            request.Success = false;
            request.Message = "error:deletedata";
            return request;
        }
        finally
        {
            if (close)
                CloseConnection(request);
        }
        return request;
    }
    public static SQL_Request TransactionUpdate(this SQL_Request request, string AppName, string XmlData, string transactionID = "", bool close = true)
    {

        request.Command = new SqlCommand();
        try
        {
            if (request.Connection.State == ConnectionState.Closed)
            { request.Connection.Open(); }
            // Create a object of SqlCommand class
            request.Command.Connection = request.Connection; //Pass the connection object to Command
            request.Command.CommandType = CommandType.StoredProcedure; // We will use stored procedure.
            request.Command.CommandText = "TransactionUpdate";
            // request.Command.CommandText = "spUpdateTimeData"; //Stored Procedure Name

            request.Command.Parameters.Add("@AppName", SqlDbType.NVarChar).Value = AppName;
            request.Command.Parameters.Add("@XmlData", SqlDbType.NVarChar).Value = XmlData;
            request.Command.Parameters.Add("@TransactionID", SqlDbType.NVarChar).Value = transactionID;

            request.Command.ExecuteNonQuery();
            request.Message = "success:assetUpdate";
        }
        catch (Exception ex)
        {
            request.Error.Ex = ex;
            request.Success = false;
            request.Message = "error:assetUpdate";
            return request;
        }
        finally
        {
            if (close)
                CloseConnection(request);
        }
        return request;
    }


}