using System.IO;
using System;
using Microsoft.AspNetCore.Mvc;
using IronPdf;
using System.Collections.Generic;
using ironpdftest.services;

namespace ironpdftest.Controllers
{
    [ApiController]
    [Route("api")]
    public class DocumentController : ControllerBase
    {
        [Route("merge")]
        [HttpPost]
        public DocumentResponse mergeDocument()
        {
           MergeService ms = new MergeService();
           return ms.Merge(this.HttpContext);
        }
    }
}