using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShippingAPI;
using System.Text;

namespace Web_App_Master
{
    public class UPS
    {
        public UPS(UPSaccount upsacct, bool testmode)
        {
            TESTMODE = testmode;
            UPSacct = upsacct;
        }

        //events
        public class SoapExceptionOccured : EventArgs
        {
            public SoapExceptionOccured(System.Web.Services.Protocols.SoapException ex = null)
            {
                if (ex != null)
                {
                    Exception = ex;
                }
            }
            public System.Web.Services.Protocols.SoapException Exception { get; set; }
        }
        public event EventHandler<SoapExceptionOccured> SoapExceptionListener;
        protected virtual void newSoapException(SoapExceptionOccured e)
        {
            try
            {
               
                EventHandler<SoapExceptionOccured> handler = SoapExceptionListener;
                if (handler != null)
                {
                    handler(this, e);
                }
            }
            catch (Exception ex) { ExceptionFire(new ExceptionOccured(ex)); }
        }
        protected virtual void ExceptionFire(ExceptionOccured obj)
        {
            EventHandler<ExceptionOccured> handler = OnException;
            if (handler != null)
            {
                handler(this, obj);
            }
        }
        public event EventHandler<ExceptionOccured> OnException;
        protected virtual void ReturnRateFire(ReturnRateEvent obj)
        {
            EventHandler<ReturnRateEvent> handler = OnRateReturn;
            if (handler != null)
            {
                handler(this, obj);
            }
        }
        public event EventHandler<ReturnRateEvent> OnRateReturn;
        protected virtual void ReturnShipFire(ReturnShipEvent obj)
        {
            EventHandler<ReturnShipEvent> handler = OnShipReturn;
            if (handler != null)
            {
                handler(this, obj);
            }
        }
        public event EventHandler<ReturnShipEvent> OnShipReturn;
        protected virtual void ReturnTransitFire(ReturnTransitEvent obj)
        {
            EventHandler<ReturnTransitEvent> handler = OnTransitReturn;
            if (handler != null)
            {
                handler(this, obj);
            }
        }
        public event EventHandler<ReturnTransitEvent> OnTransitReturn;
        public class ExceptionOccured : EventArgs
        {
            public ExceptionOccured(Exception ex = null)
            {
                if (ex != null)
                {
                    Exception = ex;
                }
            }
            public Exception Exception { get; set; }
        }
        public class ReturnRateEvent : EventArgs
        {
            public ReturnRateEvent(Rate.RESPONSE obj = null)
            {
                if (obj != null)
                {
                    Response = obj;
                }
            }
            public Rate.RESPONSE Response { get; set; }
        }
        public class ReturnShipEvent : EventArgs
        {
            public ReturnShipEvent(Ship.RESPONSE obj = null)
            {
                if (obj != null)
                {
                    Response = obj;
                }
            }
            public Ship.RESPONSE Response { get; set; }
        }
        public class ReturnTransitEvent : EventArgs
        {
            public ReturnTransitEvent(Transit.RESPONSE obj = null)
            {
                if (obj != null)
                {
                    Response = obj;
                }
            }
            public Transit.RESPONSE Response { get; set; }
        }
        //methods
        public Rate _Rate = new Rate();
        public Rate.RESPONSE _Response;
        public ShippingAPI.Ship _Ship;
        public bool TESTMODE = true;
        public UPSaccount UPSacct = new UPSaccount();

        public Ship GetShipAsync(Address shipFrom, Address shipTo, Address shipper, UPScode code, Package package, string refers = "", string refers2 = "")
        {
            if (NetInfo.CheckForInternetConnection())
            {
                try
                {

                    _Ship = new Ship(UPSacct.A,UPSacct.I,UPSacct.P,UPSacct.N);
                    _Ship.ExceptionListener += _Ship_ExceptionListener; ;
                    _Ship.ReturnListener += _Ship_ReturnListener; ;
                    _Ship.SoapExceptionListener += _Rate_SoapExceptionListener; ;
                    _Ship.Testmode = TESTMODE;
                    _Ship = _Ship.ProcessShipment(shipFrom, shipFrom, shipTo, code, package, refers, refers2);
                    return _Ship;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }return null;
        }
        public Ship GetShip(Address shipFrom, Address shipTo, Address shipper, UPScode code, Package package, string refers = "", string refers2 = "")
        {
            if (NetInfo.CheckForInternetConnection())
            {
                try
                {

                    _Ship = new Ship(UPSacct.A, UPSacct.I, UPSacct.P, UPSacct.N);
                    
                    _Ship.Testmode = TESTMODE;
                    _Ship = _Ship.ProcessShipment(shipFrom, shipFrom, shipTo, code, package, refers, refers2);
                    return _Ship;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            return null;
        }

        public Rate GetRate(Address shipFrom, Address shipTo, Address shipper, UPScode code, Package package, string option = "Rate")
        {
            if (NetInfo.CheckForInternetConnection())
            {
                try
                {


                    _Rate = new Rate(option, UPSacct.A, UPSacct.I, UPSacct.P, UPSacct.N);
                    
                    _Rate= _Rate.SubmitRateRequest(shipFrom, shipFrom, shipTo, code, package);
                    return _Rate;
                }
                catch { return null; }
            }
            return null;

        }

        public Rate GetRateAsync(Address shipFrom, Address shipTo, Address shipper, UPScode code, Package package, string option = "Rate")
        {
            if (NetInfo.CheckForInternetConnection())
            {
                try
                {


                    _Rate = new Rate(option,UPSacct.A, UPSacct.I, UPSacct.P, UPSacct.N);
                    _Rate.ExceptionListener += _Rate_ExceptionListener; 
                    _Rate.ReturnListener += _Rate_ReturnListener;
                    _Rate.SoapExceptionListener += _Rate_SoapExceptionListener;
                    _Rate.SubmitRateRequestAsync(shipFrom, shipFrom, shipTo, code, package);
                    return _Rate;
                }
                catch { return null; }
            }return null;

        }

        private void _Rate_SoapExceptionListener(object sender, ShippingAPI.SoapExceptionOccured e)
        {
            newSoapException(new SoapExceptionOccured(e.Exception));
        }

        public Transit GetTransitAsync(Address from, Address to, DateTime pickupDate, string pkgweight)
        {
            if (NetInfo.CheckForInternetConnection())
            {

                Address shipFrom = from;

                Address shipTo = to;
                try
                {
                    Transit t = new Transit();
                    //t.Initialize(ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(MainForm._UPSaccount.A)),
                    //   ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(MainForm._UPSaccount.I)),
                    //   ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(MainForm._UPSaccount.P)));
                    //t.Initialize("6D1E455E9EE5C60E", "jnoble21", "SUS12qaz");
                    t.Initialize(UPSacct.A, UPSacct.I, UPSacct.P);
                    t.AddShipFrom(shipFrom);
                    t.AddShipTo(shipTo);
                    t.AddPickup(pickupDate);
                    t.SetUnits();
                    t.SetInvoice();
                    t.SetPackage(pkgweight, "1");

                    t.ReturnListener += Estimatedarvialsucceslisten;
                    t.ExceptionListener += EstimateArrivalexlsiten;
                    t.SoapExceptionListener += _Rate_SoapExceptionListener;
                    t.EstimateAsync();
                    return t;
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }
        public Transit GetTransit(Address from, Address to, DateTime pickupDate, string pkgweight)
        {
            if (NetInfo.CheckForInternetConnection())
            {

                Address shipFrom = from;

                Address shipTo = to;
                try
                {
                    Transit t = new Transit();
                    //t.Initialize(ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(MainForm._UPSaccount.A)),
                    //   ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(MainForm._UPSaccount.I)),
                    //   ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(MainForm._UPSaccount.P)));
                    //t.Initialize("6D1E455E9EE5C60E", "jnoble21", "SUS12qaz");
                    t.Initialize(UPSacct.A, UPSacct.I, UPSacct.P);
                    t.AddShipFrom(shipFrom);
                    t.AddShipTo(shipTo);
                    t.AddPickup(pickupDate);
                    t.SetUnits();
                    t.SetInvoice();
                    t.SetPackage(pkgweight, "1");
                    
                    t.tntResponse= t.Estimate();
                    return t;
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        private void _Rate_ReturnListener(object sender, Rate.ReturnEvent e)
        {
            ReturnRateFire(new ReturnRateEvent(e.Response));
        }
        private void _Rate_ExceptionListener(object sender, ShippingAPI.ExceptionOccured e)
        {
            ExceptionFire(new ExceptionOccured(e.Exception));
        }


        private void EstimateArrivalexlsiten(object sender, Transit.ExceptionOccured e)
        {
            ExceptionFire(new ExceptionOccured(e.Exception));
        }
        private void Estimatedarvialsucceslisten(object sender, Transit.ReturnEvent e)
        {
            ReturnTransitFire(new ReturnTransitEvent(e.Response));
        }

        private void _Ship_SoapExceptionListener(object sender, SoapExceptionOccured e)
        {
            ExceptionFire(new ExceptionOccured(e.Exception));
        }
        private void _Ship_ReturnListener(object sender, Ship.ReturnEvent e)
        {
            ReturnShipFire(new ReturnShipEvent(e.Response));

        }
        private void _Ship_ExceptionListener(object sender, Ship.ExceptionOccured e)
        {
            ExceptionFire(new ExceptionOccured(e.Exception));
        }


    }

}