using IronPdf;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System;
using System.IO;

namespace ironpdftest.services
{
    public class MergeService
    {
        public DocumentResponse Merge(HttpContext httpContext)
        {
            DocumentResponse resp = new DocumentResponse();

            try
            {

                resp.IsSuccess = false;

                var files = httpContext.Request.Form.Files;

                if(files != null)
                {
                    var tempPath = Guid.NewGuid().ToString();
                    var opsPath = Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), tempPath)).ToString();
                    httpContext.Items["opsPath"] = opsPath;
                    var MergedOutput = "merged-output.pdf";
                    
                    HtmlToPdf HtmlToPdfRenderer = new HtmlToPdf();
                    HtmlToPdfRenderer.PrintOptions.PaperSize = PdfPrintOptions.PdfPaperSize.Letter;
                    var PDFs = new List<PdfDocument>();
                    
                    foreach(var file in files)
                    {
                        var filePath = Path.Combine(opsPath, file.FileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        

                        if (file.ContentType.StartsWith("image"))
                        {
                            Byte[] imageBytes = System.IO.File.ReadAllBytes(filePath);
                            string ImgDataURI = @"data:image/png;base64," + Convert.ToBase64String(imageBytes);
                            PDFs.Add(HtmlToPdfRenderer.RenderHtmlAsPdf(string.Format("<img src='{0}'>", ImgDataURI)));
                        }
                        else
                        {
                            PDFs.Add(PdfDocument.FromFile(filePath));
                        }
                    }

                    PdfDocument PDF = PdfDocument.Merge(PDFs);
                    PDF.SaveAs(Path.Combine(opsPath, MergedOutput));

                    Byte[] bytes = System.IO.File.ReadAllBytes(Path.Combine(opsPath, MergedOutput));

                    resp.DocumentContent = Convert.ToBase64String(bytes);
                    resp.IsSuccess = true;

                    Directory.Delete(opsPath, true);
                }
            }
            catch (System.Exception)
            {
                
                throw;
            }
            finally
            {
                if (httpContext.Items["opsPath"] != null && Directory.Exists(httpContext.Items["opsPath"].ToString()))
                {
                    Directory.Delete(httpContext.Items["opsPath"].ToString(), true);
                }
            }
        

            return resp;
        }
    }
}