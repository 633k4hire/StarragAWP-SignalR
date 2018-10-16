using Ghostscript.NET;
using iTextSharp.text;
using iTextSharp.text.pdf;
//using NReco.PdfRenderer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Web_App_Master.Account.Templates;
using static iTextSharp.text.pdf.AcroFields;


public class Pdf
{
    /*
    public static System.Drawing.Image ToImage(string source, string dest,int page=1)
    {
        var pdfToImg = new PdfToImageConverter();
        pdfToImg.ScaleTo = 800;        
        var memStream = new MemoryStream();
        pdfToImg.GenerateImage(source, page, ImageFormat.Png, memStream);
        System.Drawing.Bitmap bm = new System.Drawing.Bitmap(memStream);
        
        using (FileStream fs = new FileStream(dest, FileMode.Create))
        {
            memStream.Position = 0;
            memStream.CopyTo(fs);
        }
        return bm;
    }*/
    public delegate PdfStamper StampAction(PdfStamper stamper);
    public enum Alignment
    {
        Left, Center, Right
    }
    public static TextField CreateMultiLineField(float x, float y, float width, float height, string name, PdfStamper stamper, Alignment alignment = Alignment.Left)
    {
        TextField Tracking = new TextField(stamper.Writer, new iTextSharp.text.Rectangle(x, y, x + width, y + height), name);
        Tracking.Options = TextField.MULTILINE;
        try
        {
            Tracking.Alignment = (int)alignment;
        }
        catch { }
        //
        return Tracking;
    }
    public static TextField CreateField(float x, float y, float width, float height, string name, PdfStamper stamper)
    {
        TextField Tracking = new TextField(stamper.Writer, new iTextSharp.text.Rectangle(x, y, x + width, y + height), name);
        return Tracking;
    }
    public static bool CreatePdf(List<TextField> fields, string filename, string template)
    {
        try
        {
            var dest = filename;
            PdfReader reader = new PdfReader(template);
            PdfStamper stamper = new PdfStamper(reader, new FileStream(dest, FileMode.Create));
            foreach (var field in fields)
            {
                var a = field.GetTextField();

                stamper.AddAnnotation(a, 1);
            }

            stamper.Close();
            return true;
        }
        catch { return false; }
    }
    public static bool CreatePdf(List<TextField> fields, Stream output, Stream template)
    {
        try
        {

            PdfReader reader = new PdfReader(template);
            PdfStamper stamper = new PdfStamper(reader, output);
            foreach (var field in fields)
            {
                var a = field.GetTextField();

                stamper.AddAnnotation(a, 1);
            }

            stamper.Close();
            return true;
        }
        catch { return false; }
    }
    public static bool CreatePdf(List<TextField> fields, Stream output, byte[] template)
    {
        try
        {

            PdfReader reader = new PdfReader(template);
            PdfStamper stamper = new PdfStamper(reader, output);
            foreach (var field in fields)
            {
                var a = field.GetTextField();

                stamper.AddAnnotation(a, 1);
            }

            stamper.Close();
            return true;
        }
        catch { return false; }
    }
    public static bool StampPdf(StampAction stampAction, Stream output, Stream template)
    {
        try
        {
            using (PdfReader reader = new PdfReader(template))
            using (PdfStamper stamper = new PdfStamper(reader, output))
            {
               stampAction.Invoke(stamper);
            }
            return true;
        }
        catch { return false; }
    }
    public static bool StampPdf(StampAction stampAction, Stream output, string template)
    {
        try
        {
            using (PdfReader reader = new PdfReader(template))
            using (PdfStamper stamper = new PdfStamper(reader, output))
            {
                stampAction.Invoke(stamper);
            }
            return true;
        }
        catch { return false; }
    }
    public static bool StampPdf(StampAction stampAction, Stream output, byte[] template)
    {
        try
        {
            PdfReader reader = new PdfReader(template);
            PdfStamper stamper = new PdfStamper(reader, output);
            {
                stampAction.Invoke(stamper);
            }
            reader.Close();
            return true;
        }
        catch { return false; }
    }
    public static bool StampPdf(StampAction stampAction, string filename, byte[] template)
    {
        try
        {
            using (PdfReader reader = new PdfReader(template))
            using (FileStream fs = new FileStream(filename, FileMode.Create))
            using (PdfStamper stamper = new PdfStamper(reader, fs))
            {
                stampAction.Invoke(stamper);
            }




            return true;
        }
        catch { return false; }
    }
    public static bool FillInForm(string source, string dest, Dictionary<string, string> Keys)
    {
        try
        {
            using (PdfReader reader = new PdfReader(source))
            using (PdfStamper stamper = new PdfStamper(reader, new FileStream(dest, FileMode.Create)))
            {
                AcroFields form = stamper.AcroFields;
                foreach (var i in Keys)
                {
                    try
                    {
                        Item item = form.GetFieldItem(i.Key);
                        form.SetField(i.Key, i.Value);
                    }
                    catch { }
                }

            }
            return true;
        }
        catch { return false; }
    }
    public static bool FillInForm(Stream source, Stream dest, Dictionary<string, string> Keys)
    {
        try
        {

            using (PdfReader reader = new PdfReader(source))
            using (PdfStamper stamper = new PdfStamper(reader, dest))
            {
                 AcroFields form = stamper.AcroFields;
                foreach (var i in Keys)
                {
                    Item item = form.GetFieldItem(i.Key);
                    form.SetField(i.Key, i.Value);
                }
            }
            return true;
        }
        catch { return false; }
    }
    public static bool FillInForm(byte[] source, Stream dest, Dictionary<string, string> Keys)
    {
        try
        {
            PdfReader reader = new PdfReader(source);
            PdfStamper stamper = new PdfStamper(reader, dest);

            AcroFields form = stamper.AcroFields;
            foreach (var i in Keys)
            {
                Item item = form.GetFieldItem(i.Key);
                form.SetField(i.Key, i.Value);
            }

            stamper.Close();
            return true;
        }
        catch { return false; }
    }
    public static iTextSharp.text.Image createBarcode(PdfContentByte cb, String code, float mh = 1, float mw = 1, bool generateCheckSum = true)
    {
        Barcode128 pf = new Barcode128();
        pf.GenerateChecksum = (generateCheckSum);
        pf.Code = (code);
        //BarcodePDF417 pf = new BarcodePDF417();
        // pf.SetText("BarcodePDF417 barcode");
        iTextSharp.text.Rectangle size = pf.BarcodeSize;
        iTextSharp.text.pdf.PdfTemplate template = cb.CreateTemplate(mw * size.Width, mh * size.Height);
        pf.PlaceBarcode(template, BaseColor.BLACK, BaseColor.RED);
        return iTextSharp.text.Image.GetInstance(template);
    }
    public static iTextSharp.text.Image createVectorBarcode(PdfStamper stamper, String code, float mh = 1, float mw = 1, bool generateCheckSum = true, int page = 1)
    {
        Barcode128 pf = new Barcode128();
        pf.GenerateChecksum = (generateCheckSum);
        pf.Code = (code);
        //BarcodePDF417 pf = new BarcodePDF417();
        // pf.SetText("BarcodePDF417 barcode");
        iTextSharp.text.Rectangle size = pf.BarcodeSize;
        iTextSharp.text.pdf.PdfTemplate template = stamper.GetOverContent(page).CreateTemplate(mw * size.Width, mh * size.Height);
        pf.PlaceBarcode(template, BaseColor.BLACK, BaseColor.RED);
        return iTextSharp.text.Image.GetInstance(template);
    }
    public static iTextSharp.text.Image createBarcode(PdfStamper stamper, String code, float mh = 1, float mw = 1, bool generateCheckSum = true, int page = 1)
    {
        Barcode128 code128 = new Barcode128();
        code128.GenerateChecksum = (generateCheckSum);
        code128.Code = (code);
        return code128.CreateImageWithBarcode(stamper.GetOverContent(page), BaseColor.BLACK, BaseColor.RED);
    }



    public class PackingSlip
    {
        public StampAction StampAction = DefaultPackingSlipForm;
        public PackingSlip(PdfTemplate template)
        {
            Keys = DefaultPackingSlipKeys;
            Template = template;
        }
        public PdfTemplate Template=null;
        public Dictionary<string, string>  Keys;
        public static readonly Dictionary<string, string> DefaultPackingSlipKeys = new Dictionary<string, string>() {
                {"ShipDate", ""},
            {"Tracking", ""},
            {"Signature", ""},
            {"Attn", ""},
            {"Po", ""},
            {"To", ""},
            {"From", ""},
            {"Weight", ""},
            {"Ordernumber", ""},

            {"Desc1", ""},
            {"Desc2", ""},
            {"Desc3", ""},
            {"Desc4", ""},
            {"Desc5", ""},
            {"Desc6", ""},
            {"Desc7", ""},
            {"Desc8", ""},
            {"Desc9", ""},
            {"Desc10", ""},
            {"Desc11", ""},
            {"Desc12", ""},
            {"Desc13", ""},

            {"PartNo1", ""},
            {"PartNo2", ""},
            {"PartNo3", ""},
            {"PartNo4", ""},
            {"PartNo5", ""},
            {"PartNo6", ""},
            {"PartNo7", ""},
            {"PartNo8", ""},
            {"PartNo9", ""},
            {"PartNo10", ""},
            {"PartNo11", ""},
            {"PartNo12", ""},
            {"PartNo13", ""},

            {"Qty1", ""},
            {"Qty2", ""},
            {"Qty3", ""},
            {"Qty4", ""},
            {"Qty5", ""},
            {"Qty6", ""},
            {"Qty7", ""},
            {"Qty8", ""},
            {"Qty9", ""},
            {"Qty10", ""},
            {"Qty11", ""},
            {"Qty12", ""},
            {"Qty13", ""}
                    };

        public static PdfStamper DefaultPackingSlipForm(PdfStamper stamper)
        {
            try
            {
                
                List<TextField> fields = new List<TextField>();
                //botom left margin x,y

                var x = 38;
                var y = 55;
                var w = 260;
                var h = 45;
                var name = "ShipDate";
                fields.Add(Pdf.CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 54
                    ;
                w = 260;
                h = 40;
                name = "Tracking";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 45
                    ;
                w = 260;
                h = 40;
                name = "Signature";
                fields.Add(CreateField(x, y, w, h, name, stamper));
                var PartNoStartY = y;
                //field //***************************LOWER X TABLE ANGE FOR ITEMS
                x = x + 0
                    ;
                y = y + 45
                    ;
                w = 260;
                h = 18;
                name = "Desc13";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 23
                    ;
                w = 260;
                h = 18;
                name = "Desc12";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 23
                    ;
                w = 260;
                h = 18;
                name = "Desc11";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 23
                    ;
                w = 260;
                h = 18;
                name = "Desc10";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 23
                    ;
                w = 260;
                h = 18;
                name = "Desc9";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 23
                    ;
                w = 260;
                h = 18;
                name = "Desc8";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 22
                    ;
                w = 260;
                h = 18;
                name = "Desc7";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 22
                    ;
                w = 260;
                h = 18;
                name = "Desc6";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 22
                    ;
                w = 260;
                h = 18;
                name = "Desc5";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 24
                    ;
                w = 260;
                h = 18;
                name = "Desc4";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 22
                    ;
                w = 260;
                h = 18;
                name = "Desc3";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 24
                    ;
                w = 260;
                h = 18;
                name = "Desc2";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 23
                    ;
                w = 260;
                h = 18;
                name = "Desc1";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //PART NO COLUMN
                y = PartNoStartY;
                x = x + 262;
                w = 260 / 2;
                h = 18;


                x = x + 0
                    ;
                y = y + 45;

                name = "PartNo13";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 23
                    ;
                name = "PartNo12";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 23
                    ;
                name = "PartNo11";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 23
                    ;
                name = "PartNo10";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 23
                    ;
                name = "PartNo9";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 23
                    ;
                name = "PartNo8";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 22
                    ;
                name = "PartNo7";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 22
                    ;
                name = "PartNo6";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 22
                    ;
                name = "PartNo5";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 24
                    ;
                name = "PartNo4";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 22
                    ;
                name = "PartNo3";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 24
                    ;
                name = "PartNo2";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 23
                    ;
                name = "PartNo1";
                fields.Add(CreateField(x, y, w, h, name, stamper));



                //Qty COLUMN
                y = PartNoStartY;
                x = x + 130;
                w = 260 / 2;
                h = 18;


                x = x + 0
                    ;
                y = y + 45;

                name = "Qty13";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 23
                    ;
                name = "Qty12";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 23
                    ;
                name = "Qty11";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 23
                    ;
                name = "Qty10";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 23
                    ;
                name = "Qty9";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 23
                    ;
                name = "Qty8";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 22
                    ;
                name = "Qty7";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 22
                    ;
                name = "Qty6";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 22
                    ;
                name = "Qty5";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 24
                    ;
                name = "Qty4";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 22
                    ;
                name = "Qty3";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 24
                    ;
                name = "Qty2";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //field
                x = x + 0
                    ;
                y = y + 23
                    ;
                name = "Qty1";
                fields.Add(CreateField(x, y, w, h, name, stamper));


                //Attn
                //field
                w = 260;
                h = 40;
                x = 38
                    ;
                y = y + 42
                    ;
                name = "Attn";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //PO
                //field
                x = x + 262
                    ;
                y = y + 0
                    ;
                name = "Po";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //to
                //field
                w = 260;
                h = 190;
                x = 38
                    ;
                y = y + 48
                    ;
                name = "To";
                fields.Add(CreateMultiLineField(x, y, w, h, name, stamper));

                //from
                //field
                x = x + 262
                    ;
                y = y + 0
                    ;
                name = "From";
                fields.Add(CreateMultiLineField(x, y, w, h, name, stamper));

                //weight
                //field
                w = 260;
                h = 40;
                x = 38
                    ;
                y = y + 42 + 155
                    ;
                name = "Weight";
                fields.Add(CreateField(x, y, w, h, name, stamper));

                //order number
                //field
                x = x + 262
                    ;
                y = y + 0
                    ;
                name = "Ordernumber";
                var ordernum = CreateField(x, y, w, h, name, stamper);
                ordernum.Alignment = PdfAppearance.ALIGN_RIGHT;
                fields.Add(ordernum);


                //*****CHANGE ME********

                // add the field here, the second param is the page you want it on
                foreach (var field in fields)
                {
                    var a = field.GetTextField();

                    stamper.AddAnnotation(a, 1);
                }

               
            }
            catch { }
            return stamper;

        }
        public bool Create(Stream output)
        {
            return Pdf.StampPdf(this.StampAction, output, Template.TemplateBytes);
        }
        public bool Fill(Stream output, Dictionary<string, string> keys)
        {
            return Pdf.FillInForm(Template.TemplateBytes, output, keys);
        }
        public bool CreateAndFill(string file, Dictionary<string, string> keys, StampAction action = null)
        {

            var temp = HttpContext.Current.Server.MapPath( "/Account/PackingLists/"+Guid.NewGuid() + ".pdf");
            try
            {
                bool result = false;
                Pdf.PackingSlip ps = new Pdf.PackingSlip(Template);
                using (MemoryStream ms = new MemoryStream())
                {
                   
                    if (action == null)
                        result = StampPdf(this.StampAction, temp, Template.TemplateBytes);
                    else
                        result = StampPdf(action, temp, Template.TemplateBytes);

                   
                    if (result)
                    {
                        //fill data
                        using (MemoryStream fms = new MemoryStream())
                        {      
                            
                            result = Pdf.FillInForm(temp,file, keys);
                            
                        }
                       
                    }
                }
                File.Delete(temp);
                return result;
            }
            catch { return false; }
        }
        public bool CreateAndFill(Stream output, Dictionary<string, string> keys, StampAction action=null)
        {
            try
            {
                bool result = false;
                PackingSlip ps = new Pdf.PackingSlip(Template);
                if (action==null)
                    result = StampPdf(this.StampAction, output, Template.TemplateBytes);
                else
                    result = StampPdf(action, output, Template.TemplateBytes);
                if (result)
                {
                    //fill data
                    result = ps.Fill(output, keys);
                    if (result)
                    {
                        return true;

                    }
                    else
                    {
                        return false;
                    }
                }

                return true;
            }
            catch { return false; }
        }

    }
    public class Avery5160TextLabels
    {
        public Avery5160TextLabels()
        {
            Keys = DefaultAveryTextKeys;
            Template = new Avery5160TextLabelTemplate();
        }
        public Avery5160TextLabels(PdfTemplate template)
        {
            Keys = DefaultAveryTextKeys;
            Template = template;
        }

        public static PdfStamper DefaultAvery5160textLabels(PdfStamper stamper)
        {
            List<TextField> fields = new List<TextField>();
            var x = 17;
            var y = 38;
            var w = 183;
            var h = 68;
            var name = "Col1Row10";
            fields.Add(CreateMultiLineField(x, y, w, h, name, stamper));
            //Field
            x = x + 0
                ;
            y = y + 72
                ; w = 183; h = 68;
            name = "Col1Row9";
            fields.Add(CreateMultiLineField(x, y, w, h, name, stamper));
            //Field
            x = x + 0
                ;
            y = y + 72
                ; w = 183; h = 68;
            name = "Col1Row8";
            fields.Add(CreateMultiLineField(x, y, w, h, name, stamper));
            //Field
            x = x + 0
                ;
            y = y + 72
                ; w = 183; h = 68;
            name = "Col1Row7";
            fields.Add(CreateMultiLineField(x, y, w, h, name, stamper));
            //Field
            x = x + 0
                ;
            y = y + 72
                ; w = 183; h = 68;
            name = "Col1Row6";
            fields.Add(CreateMultiLineField(x, y, w, h, name, stamper));
            //Field
            x = x + 0
                ;
            y = y + 72
                ; w = 183; h = 68;
            name = "Col1Row5";
            fields.Add(CreateMultiLineField(x, y, w, h, name, stamper));
            //Field
            x = x + 0
                ;
            y = y + 72
                ; w = 183; h = 68;
            name = "Col1Row4";
            fields.Add(CreateMultiLineField(x, y, w, h, name, stamper));
            //Field
            x = x + 0
                ;
            y = y + 72
                ; w = 183; h = 68;
            name = "Col1Row3";
            fields.Add(CreateMultiLineField(x, y, w, h, name, stamper));
            //Field
            x = x + 0
                ;
            y = y + 72
                ; w = 183; h = 68;
            name = "Col1Row2";
            fields.Add(CreateMultiLineField(x, y, w, h, name, stamper));
            //Field
            x = x + 0
                ;
            y = y + 72
                ; w = 183; h = 68;
            name = "Col1Row1";
            fields.Add(CreateMultiLineField(x, y, w, h, name, stamper));

            //Next Column

            //Field
            x = x + 198
                 ;
            y = 38
                ; w = 183; h = 68;
            name = "Col2Row10";
            fields.Add(CreateMultiLineField(x, y, w, h, name, stamper));
            //Field
            x = x + 0
                ;
            y = y + 72
                ; w = 183; h = 68;
            name = "Col2Row9";
            fields.Add(CreateMultiLineField(x, y, w, h, name, stamper));
            //Field
            x = x + 0
                ;
            y = y + 72
                ; w = 183; h = 68;
            name = "Col2Row8";
            fields.Add(CreateMultiLineField(x, y, w, h, name, stamper));
            //Field
            x = x + 0
                ;
            y = y + 72
                ; w = 183; h = 68;
            name = "Col2Row7";
            fields.Add(CreateMultiLineField(x, y, w, h, name, stamper));
            //Field
            x = x + 0
                ;
            y = y + 72
                ; w = 183; h = 68;
            name = "Col2Row6";
            fields.Add(CreateMultiLineField(x, y, w, h, name, stamper));
            //Field
            x = x + 0
                ;
            y = y + 72
                ; w = 183; h = 68;
            name = "Col2Row5";
            fields.Add(CreateMultiLineField(x, y, w, h, name, stamper));
            //Field
            x = x + 0
                ;
            y = y + 72
                ; w = 183; h = 68;
            name = "Col2Row4";
            fields.Add(CreateMultiLineField(x, y, w, h, name, stamper));
            //Field
            x = x + 0
                ;
            y = y + 72
                ; w = 183; h = 68;
            name = "Col2Row3";
            fields.Add(CreateMultiLineField(x, y, w, h, name, stamper));
            //Field
            x = x + 0
                ;
            y = y + 72
                ; w = 183; h = 68;
            name = "Col2Row2";
            fields.Add(CreateMultiLineField(x, y, w, h, name, stamper));
            //Field
            x = x + 0
                ;
            y = y + 72
                ; w = 183; h = 68;
            name = "Col2Row1";
            fields.Add(CreateMultiLineField(x, y, w, h, name, stamper));

            //Next Column
            //Next Column

            //Field
            x = x + 198
                 ;
            y = 38
                ; w = 183; h = 68;
            name = "Col3Row10";
            fields.Add(CreateMultiLineField(x, y, w, h, name, stamper));
            //Field
            x = x + 0
                ;
            y = y + 72
                ; w = 183; h = 68;
            name = "Col3Row9";
            fields.Add(CreateMultiLineField(x, y, w, h, name, stamper));
            //Field
            x = x + 0
                ;
            y = y + 72
                ; w = 183; h = 68;
            name = "Col3Row8";
            fields.Add(CreateMultiLineField(x, y, w, h, name, stamper));
            //Field
            x = x + 0
                ;
            y = y + 72
                ; w = 183; h = 68;
            name = "Col3Row7";
            fields.Add(CreateMultiLineField(x, y, w, h, name, stamper));
            //Field
            x = x + 0
                ;
            y = y + 72
                ; w = 183; h = 68;
            name = "Col3Row6";
            fields.Add(CreateMultiLineField(x, y, w, h, name, stamper));
            //Field
            x = x + 0
                ;
            y = y + 72
                ; w = 183; h = 68;
            name = "Col3Row5";
            fields.Add(CreateMultiLineField(x, y, w, h, name, stamper));
            //Field
            x = x + 0
                ;
            y = y + 72
                ; w = 183; h = 68;
            name = "Col3Row4";
            fields.Add(CreateMultiLineField(x, y, w, h, name, stamper));
            //Field
            x = x + 0
                ;
            y = y + 72
                ; w = 183; h = 68;
            name = "Col3Row3";
            fields.Add(CreateMultiLineField(x, y, w, h, name, stamper));
            //Field
            x = x + 0
                ;
            y = y + 72
                ; w = 183; h = 68;
            name = "Col3Row2";
            fields.Add(CreateMultiLineField(x, y, w, h, name, stamper));
            //Field
            x = x + 0
                ;
            y = y + 72
                ; w = 183; h = 68;
            name = "Col3Row1";
            fields.Add(CreateMultiLineField(x, y, w, h, name, stamper));


            // add the field here, the second param is the page you want it on
            foreach (var field in fields)
            {
                //field.Text = ("default text");
                var a = field.GetTextField();

                stamper.AddAnnotation(a, 1);
            }

            return stamper;
        }
        public PdfTemplate Template = null;
        public Dictionary<string, string> Keys;
        public StampAction StampAction = DefaultAvery5160textLabels;
        public static readonly Dictionary<string, string> DefaultAveryTextKeys = new Dictionary<string, string>() {
            {"Col1Row1", ""},
            {"Col1Row2", ""},
            {"Col1Row3", ""},
            {"Col1Row4", ""},
            {"Col1Row5", ""},
            {"Col1Row6", ""},
            {"Col1Row7", ""},
            {"Col1Row8", ""},
            {"Col1Row9", ""},
            {"Col1Row10", ""},
            {"Col2Row1", ""},
            {"Col2Row2", ""},
            {"Col2Row3", ""},
            {"Col2Row4", ""},
            {"Col2Row5", ""},
            {"Col2Row6", ""},
            {"Col2Row7", ""},
            {"Col2Row8", ""},
            {"Col2Row9", ""},
            {"Col2Row10", ""},
            {"Col3Row1", ""},
            {"Col3Row2", ""},
            {"Col3Row3", ""},
            {"Col3Row4", ""},
            {"Col3Row5", ""},
            {"Col3Row6", ""},
            {"Col3Row7", ""},
            {"Col3Row8", ""},
            {"Col3Row9", ""},
            {"Col3Row10", ""},
                    };
        public bool Create(Stream output)
        {
            return Pdf.StampPdf(this.StampAction, output, Template.TemplateBytes);
        }
        public bool Fill(Stream output, Dictionary<string, string> keys)
        {
            return Pdf.FillInForm(Template.TemplateBytes, output, keys);
        }
        public bool CreateAndFill(string file, Dictionary<string, string> keys, StampAction action = null)
        {

            var temp = HttpContext.Current.Server.MapPath("/Account/Avery/" + Guid.NewGuid() + ".pdf");
            try
            {
                bool result = false;
                Pdf.PackingSlip ps = new Pdf.PackingSlip(Template);
                using (MemoryStream ms = new MemoryStream())
                {

                    if (action == null)
                        result = StampPdf(this.StampAction, temp, Template.TemplateBytes);
                    else
                        result = StampPdf(action, temp, Template.TemplateBytes);


                    if (result)
                    {
                        //fill data
                        using (MemoryStream fms = new MemoryStream())
                        {

                            result = Pdf.FillInForm(temp, file, keys);

                        }

                    }
                }
                File.Delete(temp);
                return result;
            }
            catch { return false; }
        }
        public bool CreateAndFill(Stream output, Dictionary<string, string> keys, StampAction action = null)
        {
            try
            {
                bool result = false;
                PackingSlip ps = new Pdf.PackingSlip(Template);
                if (action == null)
                    result = StampPdf(this.StampAction, output, Template.TemplateBytes);
                else
                    result = StampPdf(action, output, Template.TemplateBytes);
                if (result)
                {
                    //fill data
                    result = ps.Fill(output, keys);
                    if (result)
                    {
                        return true;

                    }
                    else
                    {
                        return false;
                    }
                }

                return true;
            }
            catch { return false; }
        }


    }
    public class Avery5160ImageLabels
    {
        public Avery5160ImageLabels()
        {
         
            Template = new Avery5160TextLabelTemplate();
        }
        public Avery5160ImageLabels(PdfTemplate template)
        {
           
            Template = template;
        }

        public PdfTemplate Template = null;
        public List<string> ImagePaths;
        public bool Create(Stream output)
        {

            try
            {
                PdfReader reader = new PdfReader(Template.TemplateBytes);
                PdfStamper stamper = new PdfStamper(reader, output);
                var x = 17;
                var y = 38;
                var w = 183;
                var h = 68;
                int count = 0;
                foreach (var image in ImagePaths)
                {
                    if (count == 10 || count == 20) //Reset Column
                    {
                        x = x + 198;
                        y = 38;
                    }
                    stamper = Stampers.ImageStamper(stamper, image, x, y, w, h);
                    //Set Next Row
                    y = y + 72;
                    count++;
                }

                stamper.Close();
                return true;
            }
            catch { return false; }

        }
        public bool Create(string file)
        {

            try
            {
                PdfReader reader = new PdfReader(Template.TemplateBytes);
                PdfStamper stamper = new PdfStamper(reader, File.Open(file, FileMode.Create));
                var x = 17;
                var y = 38;
                var w = 183;
                var h = 68;
                int count = 0;
                foreach (var image in ImagePaths)
                {
                    if (count == 10 || count == 20) //Reset Column
                    {
                        x = x + 198;
                        y = 38;
                    }
                    stamper = Stampers.ImageStamper(stamper, image, x, y, w, h);
                    //Set Next Row
                    y = y + 72;
                    count++;
                }

                stamper.Close();
                return true;
            }
            catch { return false; }

        }


    }
    public class Avery5160BarcodeLabels
    {
        public Avery5160BarcodeLabels()
        {

            Template = new Avery5160TextLabelTemplate();
        }
        public Avery5160BarcodeLabels(PdfTemplate template)
        {

            Template = template;
        }

        public PdfTemplate Template = null;
        public List<string> Codes;
        public bool Create(Stream output)
        {
            try
            {
                PdfReader reader = new PdfReader(Template.TemplateBytes);
                PdfStamper stamper = new PdfStamper(reader, output);
                var x = 17;
                var y = 38;
                var w = 183;
                var h = 68;
                int count = 0;
                List<iTextSharp.text.Image> images = new List<iTextSharp.text.Image>();
                foreach (var code in Codes)
                {
                    // images.Add(createBarcode(stamper, code));
                    images.Add(createVectorBarcode(stamper, code, 3, 3)); //set scale
                }

                foreach (var image in images)
                {
                    if (count == 10 || count == 20) //Reset Column
                    {
                        x = x + 198;
                        y = 38;
                    }
                    stamper = Stampers.ImageStamper(stamper, image, x, y, w, h, 3, 3); //match scale
                    //Set Next Row
                    y = y + 72;
                    count++;
                }

                stamper.Close();
                return true;
            }
            catch { return false; }
        }
        public bool Create(string file)
        {
            try
            {
                PdfReader reader = new PdfReader(Template.TemplateBytes);
                PdfStamper stamper = new PdfStamper(reader, File.Open(file, FileMode.Create));
                var x = 17;
                var y = 38;
                var w = 183;
                var h = 68;
                int count = 0;
                List<iTextSharp.text.Image> images = new List<iTextSharp.text.Image>();
                foreach (var code in Codes)
                {
                    // images.Add(createBarcode(stamper, code));
                    images.Add(createVectorBarcode(stamper, code, 3, 3)); //set scale
                }

                foreach (var image in images)
                {
                    if (count == 10 || count == 20) //Reset Column
                    {
                        x = x + 198;
                        y = 38;
                    }
                    stamper = Stampers.ImageStamper(stamper, image, x, y, w, h, 3, 3); //match scale
                    //Set Next Row
                    y = y + 72;
                    count++;
                }

                stamper.Close();
                return true;
            }
            catch { return false; }

        }


    }
    public class Avery5160GraphicLabels
    {
        public Avery5160GraphicLabels()
        {

            Template = new Avery5160TextLabelTemplate();
        }
        public Avery5160GraphicLabels(PdfTemplate template)
        {

            Template = template;
        }

        public PdfTemplate Template = null;
        public List<System.Drawing.Image> Images;
        public bool Create(Stream output)
        {

            try
            {
                PdfReader reader = new PdfReader(Template.TemplateBytes);
                PdfStamper stamper = new PdfStamper(reader, output);
                var x = 17;
                var y = 38;
                var w = 183;
                var h = 68;
                int count = 0;
                foreach (var image in Images)
                {
                    if (count == 10 || count == 20) //Reset Column
                    {
                        x = x + 198;
                        y = 38;
                    }
                    stamper = Stampers.ImageStamper(stamper, image, x, y, w, h);
                    //Set Next Row
                    y = y + 72;
                    count++;
                }

                stamper.Close();
                return true;
            }
            catch { return false; }

        }
        public bool Create(string file)
        {

            try
            {
                PdfReader reader = new PdfReader(Template.TemplateBytes);
                PdfStamper stamper = new PdfStamper(reader, File.Open(file, FileMode.Create));
                var x = 17;
                var y = 38;
                var w = 183;
                var h = 68;
                int count = 0;
                foreach (var image in Images)
                {
                    if (count == 10 || count == 20) //Reset Column
                    {
                        x = x + 198;
                        y = 38;
                    }
                    stamper = Stampers.ImageStamper(stamper, image, x, y, w, h);
                    //Set Next Row
                    y = y + 72;
                    count++;
                }

                stamper.Close();
                return true;
            }
            catch { return false; }

        }


    }

    public class Stampers
    {
        public static PdfStamper ImageStamper(PdfStamper stamper, System.Drawing.Image img, float x, float y, float w, float h, int scalex = 1, int scaley = 1, int page = 1)
        {

            var pdfContentByte = stamper.GetOverContent(page);
            try
            {
                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(img, System.Drawing.Imaging.ImageFormat.Bmp);
                image.ScaleAbsolute(w * scalex, h * scaley);
                image.SetAbsolutePosition(x, y);
                pdfContentByte.AddImage(image);
            }
            catch { }
            return stamper;
        }

        public static PdfStamper ImageStamper(PdfStamper stamper, string img, float x, float y, float w, float h, int scalex=1, int scaley=1 , int page = 1)
        {
            using (Stream inputImageStream = new FileStream(img, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var pdfContentByte = stamper.GetOverContent(page);
                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(inputImageStream);
                image.ScaleAbsolute(w*scalex, h*scaley);
                image.SetAbsolutePosition(x, y);
                pdfContentByte.AddImage(image);
            }
            return stamper;
        }
        public static PdfStamper ImageStamper(PdfStamper stamper, iTextSharp.text.Image image, float x, float y, float w, float h, int scalex = 1, int scaley = 1, int page = 1)
        {
                var pdfContentByte = stamper.GetOverContent(page);
                image.ScaleAbsolute(w * scalex, h * scaley);
                image.SetAbsolutePosition(x, y);
                pdfContentByte.AddImage(image);            
            return stamper;
        }

    }
    public class PdfTemplate
    {
        public string m_template = "JVBERi0xLjUNCiW1tbW1DQoxIDAgb2JqDQo8PC9UeXBlL0NhdGFsb2cvUGFnZXMgMiAwIFIvTGFuZyhlbi1VUykgL1N0cnVjdFRyZWVSb290IDcgMCBSL01hcmtJbmZvPDwvTWFya2VkIHRydWU+Pj4+DQplbmRvYmoNCjIgMCBvYmoNCjw8L1R5cGUvUGFnZXMvQ291bnQgMS9LaWRzWyAzIDAgUl0gPj4NCmVuZG9iag0KMyAwIG9iag0KPDwvVHlwZS9QYWdlL1BhcmVudCAyIDAgUi9SZXNvdXJjZXM8PC9Gb250PDwvRjEgNSAwIFI+Pi9Qcm9jU2V0Wy9QREYvVGV4dC9JbWFnZUIvSW1hZ2VDL0ltYWdlSV0gPj4vTWVkaWFCb3hbIDAgMCA1OTUuMzIgODQxLjkyXSAvQ29udGVudHMgNCAwIFIvR3JvdXA8PC9UeXBlL0dyb3VwL1MvVHJhbnNwYXJlbmN5L0NTL0RldmljZVJHQj4+L1RhYnMvUy9TdHJ1Y3RQYXJlbnRzIDA+Pg0KZW5kb2JqDQo0IDAgb2JqDQo8PC9MZW5ndGggMTAxPj4NCnN0cmVhbQ0KIC9QIDw8L01DSUQgMC9MYW5nIChkZS1DSCk+PiBCREMgQlQNCi9GMSA5Ljk2IFRmDQoxIDAgMCAxIDM2IDc5Ni42OCBUbQ0KMCBnDQowIEcNClsoICldIFRKDQpFVA0KIEVNQyANCmVuZHN0cmVhbQ0KZW5kb2JqDQo1IDAgb2JqDQo8PC9UeXBlL0ZvbnQvU3VidHlwZS9UcnVlVHlwZS9OYW1lL0YxL0Jhc2VGb250L0FyaWFsTVQvRW5jb2RpbmcvV2luQW5zaUVuY29kaW5nL0ZvbnREZXNjcmlwdG9yIDYgMCBSL0ZpcnN0Q2hhciAzMi9MYXN0Q2hhciAzMi9XaWR0aHMgMTUgMCBSPj4NCmVuZG9iag0KNiAwIG9iag0KPDwvVHlwZS9Gb250RGVzY3JpcHRvci9Gb250TmFtZS9BcmlhbE1UL0ZsYWdzIDMyL0l0YWxpY0FuZ2xlIDAvQXNjZW50IDkwNS9EZXNjZW50IC0yMTAvQ2FwSGVpZ2h0IDcyOC9BdmdXaWR0aCA0NDEvTWF4V2lkdGggMjY2NS9Gb250V2VpZ2h0IDQwMC9YSGVpZ2h0IDI1MC9MZWFkaW5nIDMzL1N0ZW1WIDQ0L0ZvbnRCQm94WyAtNjY1IC0yMTAgMjAwMCA3MjhdID4+DQplbmRvYmoNCjEzIDAgb2JqDQo8PC9UeXBlL09ialN0bS9OIDYvRmlyc3QgMzYvRmlsdGVyL0ZsYXRlRGVjb2RlL0xlbmd0aCAyODQ+Pg0Kc3RyZWFtDQp4nG1RwYrCMBC9C/7D/MEk3YoVRJBV2UUspS3sQTzEdrYttonEFPTvN2kr7WEvybyZ995MJpwDA85gwWFpbwYB8GAFK/C9BXAPfD+A9Rojx2EQY4IRpq87YWJ0m5l9TQ0ez8AugFEBH46z2cxnnWT5Vght/hNx1zu+wCCYMFJNFCtlMFY1ncTdTuWsrBHJrmgHdAlnwnqTSTGkpznSC/hgfLBOUhnC0B17mY8gtdSremJCmcEvEjnpPnaad/wt60pSUgo3n0tspXUQplJywNpUv8IGHfpR+nZV6oY7lbWNnanLPEoi06/iJDKtJviztOcE7ypRq2KSSOoqpwm372NphRYNHqqi1TS8NWybx9l9qTfudtz0fPYHJuqasw0KZW5kc3RyZWFtDQplbmRvYmoNCjE0IDAgb2JqDQo8PC9Qcm9kdWNlcij+/wBNAGkAYwByAG8AcwBvAGYAdACuACAAVwBvAHIAZAAgADIAMAAxADMpIC9DcmVhdG9yKP7/AE0AaQBjAHIAbwBzAG8AZgB0AK4AIABXAG8AcgBkACAAMgAwADEAMykgL0NyZWF0aW9uRGF0ZShEOjIwMTgwMTE1MjEwMTM4LTA4JzAwJykgL01vZERhdGUoRDoyMDE4MDExNTIxMDEzOC0wOCcwMCcpID4+DQplbmRvYmoNCjE1IDAgb2JqDQpbIDI3OF0gDQplbmRvYmoNCjE2IDAgb2JqDQo8PC9UeXBlL1hSZWYvU2l6ZSAxNi9XWyAxIDQgMl0gL1Jvb3QgMSAwIFIvSW5mbyAxNCAwIFIvSURbPDBERDNGMTgzNzJBQTU2NDFBQkM0N0MxRDQyOUMwRDJGPjwwREQzRjE4MzcyQUE1NjQxQUJDNDdDMUQ0MjlDMEQyRj5dIC9GaWx0ZXIvRmxhdGVEZWNvZGUvTGVuZ3RoIDY3Pj4NCnN0cmVhbQ0KeJxjYACC//8ZgaQgAwOIqoFQW8AU4wIwxWQDoe4AMVCOl4EJQjFDKBYIxQihoEpYgRqY94H1sdqBKTYuBgYA5kMGgw0KZW5kc3RyZWFtDQplbmRvYmoNCnhyZWYNCjAgMTcNCjAwMDAwMDAwMDcgNjU1MzUgZg0KMDAwMDAwMDAxNyAwMDAwMCBuDQowMDAwMDAwMTI0IDAwMDAwIG4NCjAwMDAwMDAxODAgMDAwMDAgbg0KMDAwMDAwMDQxNiAwMDAwMCBuDQowMDAwMDAwNTcyIDAwMDAwIG4NCjAwMDAwMDA3MzIgMDAwMDAgbg0KMDAwMDAwMDAwOCA2NTUzNSBmDQowMDAwMDAwMDA5IDY1NTM1IGYNCjAwMDAwMDAwMTAgNjU1MzUgZg0KMDAwMDAwMDAxMSA2NTUzNSBmDQowMDAwMDAwMDEyIDY1NTM1IGYNCjAwMDAwMDAwMTMgNjU1MzUgZg0KMDAwMDAwMDAwMCA2NTUzNSBmDQowMDAwMDAxMzQyIDAwMDAwIG4NCjAwMDAwMDE1NDYgMDAwMDAgbg0KMDAwMDAwMTU3MyAwMDAwMCBuDQp0cmFpbGVyDQo8PC9TaXplIDE3L1Jvb3QgMSAwIFIvSW5mbyAxNCAwIFIvSURbPDBERDNGMTgzNzJBQTU2NDFBQkM0N0MxRDQyOUMwRDJGPjwwREQzRjE4MzcyQUE1NjQxQUJDNDdDMUQ0MjlDMEQyRj5dID4+DQpzdGFydHhyZWYNCjE4NDANCiUlRU9GDQp4cmVmDQowIDANCnRyYWlsZXINCjw8L1NpemUgMTcvUm9vdCAxIDAgUi9JbmZvIDE0IDAgUi9JRFs8MEREM0YxODM3MkFBNTY0MUFCQzQ3QzFENDI5QzBEMkY+PDBERDNGMTgzNzJBQTU2NDFBQkM0N0MxRDQyOUMwRDJGPl0gL1ByZXYgMTg0MC9YUmVmU3RtIDE1NzM+Pg0Kc3RhcnR4cmVmDQoyMzM2DQolJUVPRg==";
        public byte[] TemplateBytes
        {
            get
            {
                return Convert.FromBase64String(m_template);
            }
        }
        public string TemplateBase64 { get { return m_template; } set { m_template = value; } }
    }

    public static void ToImage(string source)
    {
        string filenoext = Path.GetFileNameWithoutExtension(source);
        string directory = source.Replace(Path.GetFileName(source), "");
        var dest = directory + filenoext + ".png";
        var bb = 0;
        GhostscriptPngDevice img = new GhostscriptPngDevice(GhostscriptPngDeviceType.Png16m);
        img.GraphicsAlphaBits = GhostscriptImageDeviceAlphaBits.V_4;
        img.TextAlphaBits = GhostscriptImageDeviceAlphaBits.V_4;
        img.ResolutionXY = new GhostscriptImageDeviceResolution(800, 600);
        img.InputFiles.Add(source);
        img.Pdf.FirstPage = 1;
        img.Pdf.LastPage = 1;
        img.PostScript = String.Empty;
        img.OutputPath = dest;
        img.Process();
    }

    public static void CombineMultiplePDFs(string[] fileNames, string outFile)
    {
        // step 1: creation of a document-object
        Document document = new Document();

        // step 2: we create a writer that listens to the document
        PdfCopy writer = new PdfCopy(document, new FileStream(outFile, FileMode.Create));
        if (writer == null)
        {
            return;
        }

        // step 3: we open the document
        document.Open();

        foreach (string fileName in fileNames)
        {
            // we create a reader for a certain document
            PdfReader reader = new PdfReader(fileName);
            reader.ConsolidateNamedDestinations();

            // step 4: we add content
            for (int i = 1; i <= reader.NumberOfPages; i++)
            {
                PdfImportedPage page = writer.GetImportedPage(reader, i);
                writer.AddPage(page);
            }

            PRAcroForm form = reader.AcroForm;
            if (form != null)
            {
                writer.CopyDocumentFields(reader);
            }

            reader.Close();
        }

        // step 5: we close the document and writer
        writer.Close();
        document.Close();
    }
    public static void CombineMultiplePDFs(List<byte[]> bytelist, string outFile)
    {
        // step 1: creation of a document-object
        Document document = new Document();

        // step 2: we create a writer that listens to the document
        PdfCopy writer = new PdfCopy(document, new FileStream(outFile, FileMode.Create));
        if (writer == null)
        {
            return;
        }

        // step 3: we open the document
        document.Open();

        foreach (var bytes in bytelist)
        {
            // we create a reader for a certain document
            PdfReader reader = new PdfReader(bytes);
            reader.ConsolidateNamedDestinations();

            // step 4: we add content
            for (int i = 1; i <= reader.NumberOfPages; i++)
            {
                PdfImportedPage page = writer.GetImportedPage(reader, i);
                writer.AddPage(page);
            }

            PRAcroForm form = reader.AcroForm;
            if (form != null)
            {
                writer.CopyDocumentFields(reader);
            }

            reader.Close();
        }

        // step 5: we close the document and writer
        writer.Close();
        document.Close();
    }
    public static void CombineMultiplePDFs(List<byte[]> bytelist, Stream output)
    {
        // step 1: creation of a document-object
        Document document = new Document();

        // step 2: we create a writer that listens to the document
        PdfCopy writer = new PdfCopy(document, output);
        if (writer == null)
        {
            return;
        }

        // step 3: we open the document
        document.Open();

        foreach (var bytes in bytelist)
        {
            // we create a reader for a certain document
            PdfReader reader = new PdfReader(bytes);
            reader.ConsolidateNamedDestinations();

            // step 4: we add content
            for (int i = 1; i <= reader.NumberOfPages; i++)
            {
                PdfImportedPage page = writer.GetImportedPage(reader, i);
                writer.AddPage(page);
            }

            PRAcroForm form = reader.AcroForm;
            if (form != null)
            {
                writer.CopyDocumentFields(reader);
            }

            reader.Close();
        }

        // step 5: we close the document and writer
        writer.Close();
        document.Close();
    }

    public static bool ImageToPdf(string source, string dest)
    {
   
        Document doc = new Document(PageSize.A4, 10f, 10f, 10f, 10f);

        PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(source, FileMode.Create));

        doc.Open();

        try

        {
            
            Paragraph paragraph = new Paragraph("Getting Started ITextSharp.");

            string imageURL = HttpContext.Current.Server.MapPath(".") + "/image2.jpg";

            iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageURL);

            //Resize image depend upon your need

            jpg.ScaleToFit(140f, 120f);

            //Give space before image

            jpg.SpacingBefore = 10f;

            //Give some space after the image

            jpg.SpacingAfter = 1f;

            jpg.Alignment = Element.ALIGN_LEFT;



            doc.Add(paragraph);

            doc.Add(jpg);



        }

        catch (Exception ex)

        { return false; }

        finally

        {

            doc.Close();
           
        }
        return true;
    
    }
    public static System.Drawing.Size ResizeKeepAspect(System.Drawing.Size src, decimal maxWidth, decimal maxHeight)
    {
        decimal rnd = Math.Min(maxWidth / (decimal)src.Width, maxHeight / (decimal)src.Height);
        return new System.Drawing.Size((int)Math.Round(src.Width * rnd), (int)Math.Round(src.Height * rnd));
    }
    public static bool ImageToPdf(System.Drawing.Image image, string dest)
    {
        try
        {

            using (Document document = new Document(PageSize.A4))
            {
                try
                {

                    var pagesize = document.PageSize;
                    var width = pagesize.Width;
                    var height = pagesize.Height;
                    //image = ResizeImage(image, width, 600, InterpolationMode.HighQualityBicubic, true);
                    var newsize = ResizeKeepAspect(image.Size, Convert.ToDecimal(width), Convert.ToDecimal(width / 2));
                    PdfWriter.GetInstance(document, new FileStream(dest, FileMode.Create));
                    document.Open();
                    iTextSharp.text.Image pdfImage = iTextSharp.text.Image.GetInstance(image, System.Drawing.Imaging.ImageFormat.Jpeg);
                    pdfImage.ScaleAbsolute(new iTextSharp.text.Rectangle(newsize.Width, newsize.Height));
                    document.Add(pdfImage);
                    document.Close();
                }
                catch
                {
                    document.Close();
                    return false;
                }
            }
            return true;
        }
        catch { return false; }
    }
    public static bool Merge(List<String> InFiles, String OutFile, bool flatten=false)
    {
        try
        {
            using (FileStream stream = new FileStream(OutFile, FileMode.Create))
            using (Document doc = new Document())
            using (PdfCopy pdf = new PdfCopy(doc, stream))
            {
                doc.Open();

                PdfReader reader = null;
                PdfImportedPage page = null;

                //fixed typo
                InFiles.ForEach(file =>
                {
                    if (flatten)
                    {
                        Flatten(file, file);
                    }
                    
                    reader = new PdfReader(file);

                    for (int i = 0; i < reader.NumberOfPages; i++)
                    {
                        page = pdf.GetImportedPage(reader, i + 1);
                        pdf.AddPage(page);
                    }

                    pdf.FreeReader(reader);
                    reader.Close();
                });
            }
            return true;
        }
        catch { return false; }
    }
    public static bool Flatten(string filename, string dest)
    {
        try
        {
            PdfReader reader = new PdfReader(filename);
            using (MemoryStream ms = new MemoryStream())
            {
                PdfStamper stamper = new PdfStamper(reader, ms);
                //Method 1
                stamper.FormFlattening = true;
                //Method 1
                // string[] fields = stamper.AcroFields.Fields.Select(x => x.Key).ToArray();
                // for (int key = 0; key <= fields.Count() - 1; key++)
                //  {
                //       stamper.AcroFields.SetFieldProperty(fields(key), "setfflags", PdfFormField.FF_READ_ONLY, null);
                //   }
                stamper.Writer.CloseStream = false;
                stamper.Close();
                reader.Close();
                using (FileStream fs = new FileStream(dest, FileMode.Create))
                {
                    ms.Position = 0;
                    ms.CopyTo(fs);
                }

            }


            return true;
        }
        catch { return false; }
    }

   
}

public static class PdfExtensions
{
    public static PdfStamper StampImage(this PdfStamper stamper, string img, float x, float y, float w, float h, int page = 1)
    {
        using (Stream inputImageStream = new FileStream(img, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            var pdfContentByte = stamper.GetOverContent(page);
            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(inputImageStream);
            image.ScaleAbsolute(w, h);
            image.SetAbsolutePosition(x, y);
            pdfContentByte.AddImage(image);
        }
        return stamper;
    }
}
