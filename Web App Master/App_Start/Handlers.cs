using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using Helpers;
using iTextSharp.text.pdf;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Drawing.Drawing2D;
using Web_App_Master.Account;
using System.Text;

namespace Web_App_Master
{
    public class BarcodeHandler : IHttpHandler
    {
        //some stuff snipped
        public bool IsReusable
        {
            get { return false; }
        }
        public void SomeCheckImageHandler() { }

        public void ProcessRequest(HttpContext context)
        {
            string prodCode = context.Request.QueryString.Get("code");
            var w = context.Request.QueryString.Get("w");
            var h = context.Request.QueryString.Get("h");
            int width=320;
            int height=100;
            if (w!=null)
            {
                try { width = Convert.ToInt32(w); } catch { width = 320; }
            }
            if (h != null)
            {
                try { height = Convert.ToInt32(h); } catch { height = 100; }
            }

            var filename = context.Request.Url.Segments.Last().Replace(".barcode","").Replace(".Barcode", "").Replace(".BARCODE", "");
            context.Response.ContentType = "image/png";
            if (prodCode.Length > 0)
            {
                if (prodCode.Length > Global.Library.Settings.AssetNumberLength) prodCode = prodCode.Substring(0, Global.Library.Settings.AssetNumberLength);
                Barcode128 code128 = new Barcode128();
                code128.CodeType = Barcode.CODE128;
                code128.ChecksumText = true;
                code128.GenerateChecksum = true;
                code128.StartStopText = true;
                code128.Code = prodCode;
                System.Drawing.Bitmap bm = new System.Drawing.Bitmap(code128.CreateDrawingImage(System.Drawing.Color.Black, System.Drawing.Color.White));
                Bitmap resized = new Bitmap(bm, new Size(width,height));
             
                resized.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Png);
            }

        }
    }
    public class ResizeHandler : IHttpHandler
    {
        //some stuff snipped
        public bool IsReusable
        {
            get { return false; }
        }
        public void SomeCheckImageHandler() { }

        public void ProcessRequest(HttpContext context)
        {
            string file = "~/Images/transparent.png";
            //var file = Encoding.UTF8.GetString( HttpServerUtility.UrlTokenDecode(context.Request["f"]));
            var height = Convert.ToInt32( context.Request["h"]);
            var width = Convert.ToInt32( context.Request["w"]);
            file = context.Request.Path.Replace(".r", "");
            file = HttpContext.Current.Server.MapPath(file);
            context.Response.ContentType = "image/png";
            try
            {
                Image img = null;
                try
                {
                    img = Image.FromFile(file);
                }
                catch {
                    try
                    {
                        img = Image.FromFile(HttpContext.Current.Server.MapPath("/Images/transparent.png"));
                    }
                    catch
                    {
                        img = new Bitmap(10, 10);
                    }
                }
                Image _img = new Bitmap(width, height);

                Graphics graphics = Graphics.FromImage(_img);



                //Resize picture according to size

                graphics.DrawImage(img, 0, 0, width, height);

                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;

                graphics.Dispose();

                img.Dispose();



                //Create outpur sream

                MemoryStream str = new MemoryStream();

                _img = _img.GetThumbnailImage(width, height, null, IntPtr.Zero);

                _img.Save(str, System.Drawing.Imaging.ImageFormat.Png);

                _img.Dispose();

                str.WriteTo(context.Response.OutputStream);

                str.Dispose();

                str.Close();

                //Set response type

                context.Response.ContentType = ".jpg";

                context.Response.End();



                




            }
            catch { }
        }
    }

    public class ImageHandler : IHttpHandler
    {
        //some stuff snipped
        public bool IsReusable
        {
            get { return false; }
        }
        public void SomeCheckImageHandler() { }

        public void ProcessRequest(HttpContext context)
        {

            var id = context.Request["Id"];
            context.Response.ContentType = "image/png";
            var idx = context.Request["idx"];
            if (idx != null)
            {
                var apth = context.Server.MapPath(id + "/");
                if (Directory.Exists(apth))
                {
                    string[] filePaths = System.IO.Directory.GetFiles(apth);
                    List<Image> files = new List<Image>();
                    foreach (string filePath in filePaths)
                    {
                        string fileName = System.IO.Path.GetFileName(filePath);
                        var ext = System.IO.Path.GetExtension(fileName).ToUpper();
                        if (ext.Equals(".PNG"))
                        {
                            files.Add(Image.FromFile(filePath));
                        }

                    }
                    files[Convert.ToInt32(idx)].Save(context.Response.OutputStream, ImageFormat.Png);
                }
            }

        }
    }
    public class PdfHandler : IHttpHandler
    {
        //some stuff snipped
        public bool IsReusable
        {
            get { return false; }
        }
        public void SomeCheckImageHandler() { }

        public void ProcessRequest(HttpContext context)
        {  
            var path = context.Request["path"];

            Byte[] buffer = File.ReadAllBytes(path);

            if (buffer != null)
            {
                context.Response.ContentType = "application/pdf";
                context.Response.AddHeader("content-length", buffer.Length.ToString());
                context.Response.BinaryWrite(buffer);
            }

        }
    }
    public class PdfLabelHandler : IHttpHandler
    {
        //some stuff snipped
        public bool IsReusable
        {
            get { return false; }
        }
        public void ReturnBlank(HttpContext context)
        {
            var buffer = new Pdf.PdfTemplate().TemplateBytes;
            context.Response.ContentType = "application/pdf";
            context.Response.AddHeader("content-length", buffer.Length.ToString());
            context.Response.BinaryWrite(buffer);

        }
        public void ReturnPdf(HttpContext context, byte[] buffer)
        {
            context.Response.ContentType = "application/pdf";
            context.Response.AddHeader("content-length", buffer.Length.ToString());
            context.Response.BinaryWrite(buffer);

        }
        private void DrawRotatedTextAt(Graphics gr, float angle, string txt, int x, int y, Font the_font, Brush the_brush)
        {
            // Save the graphics state.
            GraphicsState state = gr.Save();
            gr.ResetTransform();

            // Rotate.
            gr.RotateTransform(angle);

            // Translate to desired position. Be sure to append
            // the rotation so it occurs after the rotation.
            gr.TranslateTransform(x, y, MatrixOrder.Append);

            // Draw the text at the origin.
            gr.DrawString(txt, the_font, the_brush, 0, 0);

            // Restore the graphics state.
            gr.Restore(state);
        }
        public void ProcessRequest(HttpContext context)
        {
            string id = Path.GetFileNameWithoutExtension(context.Request.Path);
            try
            {
               

                if (id != null)
                {
                    if (id == "")
                    {
                        //context.Application["CurrentPrintList" + id] = null;
                        ReturnBlank(context);
                        return;
                    }
                    object obj = null;
                    try
                    {
                        obj = context.Application["CurrentPrintList" + id];
                    }
                    catch { }

                    if (obj != null)
                    {
                        if (obj is List<string>)
                        {
                            var list = obj as List<string>;
                            if (list.Count > 0)
                            {
                                List<byte[]> Buffers = new List<byte[]>();
                                var pages = list.SplitList(30);

                                foreach (var page in pages)
                                {
                                    //create images
                                    List<Image> images = new List<Image>();
                                    foreach (var code in page)
                                    {
                                        try
                                        {
                                            var asset = Global.AssetCache.FindAssetByNumber(code);
                                            Bitmap bmp = new Bitmap(263, 100);
                                            RectangleF rectf = new RectangleF(10, 40, 248, 60);
                                            Graphics g = Graphics.FromImage(bmp);
                                            
                                            g.SmoothingMode = SmoothingMode.AntiAlias;
                                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                                            g.FillRectangle(Brushes.White, new System.Drawing.Rectangle(0, 0, 263, 100));
                                            //DrawRotatedTextAt(g, 90, asset.AssetNumber, 0, 0, new System.Drawing.Font("Tahoma", 21, FontStyle.Bold), Brushes.Black);
                                            g.DrawString(asset.AssetName, new System.Drawing.Font("Tahoma", 11, FontStyle.Regular), Brushes.Black, rectf);
                                            g.DrawString(asset.AssetNumber, new System.Drawing.Font("Tahoma", 23, FontStyle.Bold), Brushes.Black, 2,2);
                                            g.ResetTransform();
                                            g.Flush();
                                            images.Add(bmp);
                                        }
                                        catch {
                                            ReturnBlank(context);
                                            return;
                                        }
                                    }

                                    Pdf.Avery5160GraphicLabels label = new Pdf.Avery5160GraphicLabels();
                                    label.Images = images;
                                    using (MemoryStream ms = new MemoryStream())
                                    {
                                        var result = label.Create(ms);
                                        if (!result)
                                        {
                                            ReturnBlank(context);
                                            return;
                                        }
                                        //ms.Position = 0;
                                        byte[] buffer = ms.ToArray();
                                        Buffers.Add(buffer);

                                    }
                                }
                                if (Buffers.Count == 0)
                                {
                                    //context.Application["CurrentPrintList" + id] = null;
                                    ReturnBlank(context);
                                    return;
                                }
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    Pdf.CombineMultiplePDFs(Buffers, ms);
                                    //ms.Position = 0;
                                    ReturnPdf(context, ms.ToArray());
                                    return;
                                }

                            }
                            else
                            {
                                //context.Application["CurrentPrintList" + id] = null;
                                ReturnBlank(context);
                                return;
                            }
                        }
                        else
                        {
                           // context.Application["CurrentPrintList" + id] = null;
                            ReturnBlank(context);
                            return;
                        }
                    }
                    else
                    {
                        //context.Application["CurrentPrintList" + id] = null;
                        ReturnBlank(context);
                        return;
                    }
                }
                else
                {
                   // context.Application["CurrentPrintList" + id] = null;
                    ReturnBlank(context);
                    return;
                }
            }
            catch
            {
               // context.Application["CurrentPrintList" + id] = null;
                ReturnBlank(context);
                return;
            }
        }
    }
    public class PdfBarcodeHandler : IHttpHandler
    {
        //some stuff snipped
        public bool IsReusable
        {
            get { return false; }
        }
        public void ReturnBlank(HttpContext context)
        {
            var buffer = new Pdf.PdfTemplate().TemplateBytes;
            context.Response.ContentType = "application/pdf";
            context.Response.AddHeader("content-length", buffer.Length.ToString());
            context.Response.BinaryWrite(buffer);

        }
        public void ReturnPdf(HttpContext context, byte[] buffer)
        {
            context.Response.ContentType = "application/pdf";
            context.Response.AddHeader("content-length", buffer.Length.ToString());
            context.Response.BinaryWrite(buffer);
            
        }
        public void ProcessRequest(HttpContext context)
        {
             string id = Path.GetFileNameWithoutExtension(context.Request.Path);
            try
            {

               

                if (id != null)
                {
                    if (id == "")
                    {
                        //context.Application["CurrentPrintList" + id] = null;
                        ReturnBlank(context);
                        return;
                    }
                    object obj = null;
                    try
                    {
                        obj = context.Application["CurrentPrintList" + id];
                    }
                    catch { }

                    if (obj != null)
                    {
                        if (obj is List<string>)
                        {
                            var list = obj as List<string>;
                            if (list.Count > 0)
                            {
                                List<byte[]> Buffers = new List<byte[]>();
                                var pages = list.SplitList(30);

                                foreach (var page in pages)
                                {
                                    Pdf.Avery5160BarcodeLabels label = new Pdf.Avery5160BarcodeLabels();
                                    label.Codes = page;
                                    using (MemoryStream ms = new MemoryStream())
                                    {
                                        var result = label.Create(ms);
                                        if (!result)
                                        {
                                            ReturnBlank(context);
                                            return;
                                        }
                                        //ms.Position = 0;
                                        byte[] buffer = ms.ToArray();
                                        Buffers.Add(buffer);

                                    }
                                }
                                if (Buffers.Count == 0)
                                {
                                    //context.Application["CurrentPrintList" + id] = null;
                                    ReturnBlank(context);
                                    return;
                                }
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    Pdf.CombineMultiplePDFs(Buffers, ms);
                                    //ms.Position = 0;
                                    ReturnPdf(context, ms.ToArray());
                                    return;
                                }

                            }
                            else
                            {
                                //context.Application["CurrentPrintList" + id] = null;
                                ReturnBlank(context);
                                return;
                            }
                        }
                        else
                        {
                           // context.Application["CurrentPrintList" + id] = null;
                            ReturnBlank(context);
                            return;
                        }
                    }
                    else
                    {
                       // context.Application["CurrentPrintList" + id] = null;
                        ReturnBlank(context);
                        return;
                    }
                }
                else
                {
                   // context.Application["CurrentPrintList" + id] = null;
                    ReturnBlank(context);
                    return;
                }
            }
            catch
            {
                //context.Application["CurrentPrintList" + id] = null;
                ReturnBlank(context);
                return;
            }

        }
    }


}