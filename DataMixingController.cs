using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DevExtreme.AspNet.Data;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using DataDictionaryWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;
using DataDictionary.Entities;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using DataDictionary.Models.ResponseModels;
using ClosedXML.Excel;
using Microsoft.Office.Interop.Excel;

namespace DataDictionary.Controllers
{
  
    
    public class DataMixingController : Controller

    {
        private IHostingEnvironment _environment;
        private EahouseContext _context;
        private IDataMixingContract _datamixing;

        public DataMixingController(EahouseContext context, IDataMixingContract datamixing, IHostingEnvironment environment)
        {
            _context = context;
            _datamixing = datamixing;
            _environment = environment;
        }
        [HttpGet]
       
        public async Task<ActionResult> Get()
        {

            var datamixing = await _context.DataMixing.ToListAsync();
            return Ok(datamixing);
        }
        [HttpPost]
        public List<DataMixingRequestModel> GetDetailDataByVersion(DataMixingMasterIdRequestModel version)
        {
            return _datamixing.GetDetailDataByVersion(version);
        }




        [HttpPost]
        public List<DataMixingMasterRequestModel> GetDistinct()
        {
            return _datamixing.GetDistinct();
        }
     

        [HttpPost]
        public List<ApprovalStatusNameRequest> GetApproval()
        {
            return _datamixing.GetApproval();
        }

        [HttpPost]
        public List<DataMixingMasterRequestModel> GetDistinctbyId(GetByDataMixingMasterIdRequestModel IdDataMixing)
        {
            return _datamixing.GetDistinctbyId(IdDataMixing);
        }
        [HttpPost]
        public List<DataMixingApprovalStatus> GetApprovalStatusbyId(DataMixingApprovalStatus IdDataMixing)
        {
            return _datamixing.GetApprovalStatusbyId(IdDataMixing);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public void UpdateDataMixingMaster([FromBody]DataMixingMasterUpdate newData)
        {
            _datamixing.UpdateDataMixingMaster(newData);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public void UpdateDataMixingMasterStatus([FromBody]DataMixingMasterUpdate newData)
        {
            _datamixing.UpdateDataMixingMasterStatus(newData);

        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddDataMixingMaster([FromBody]DataMixingMaster newData)
        {

            var result = _datamixing.AddDataMixingMaster(newData);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }

        [HttpPost]
        public ActionResult Add([FromBody]DataMixing datamixing)
        {


            _datamixing.Add(datamixing);

            _datamixing.SaveAll();

            return Ok(datamixing);
        }



        [Authorize(Roles = "Admin")]
        [HttpPost]

        public void UpdateDataMixingDetail([FromBody]DataMixingDetailUpdateModel newData)
        {
            _datamixing.UpdateDataMixingDetail(newData);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]

        public void DeleteDataMixing([FromBody]DataMixingDictionaryIdRequestModel dataMixingDictionaryIdRequestModel)
        {
            _datamixing.DeleteDataMixing(dataMixingDictionaryIdRequestModel);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public void DeleteDataMixingMaster([FromBody]DataMixingMaster newData)
        {
            _datamixing.DeleteDataMixingMaster(newData);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult UpdateDataWithDataDictionary([FromBody]DataMixingMasterIdRequestModel newData)
        {
            var result = _datamixing.UpdateDataWithDataDictionary(newData);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest();

        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public List<KeyValueResponseModel> GetDataMixingScopeList()
        {
            return _datamixing.GetDataMixingScopeList();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddDataMixingScope([FromBody]AddParameterModel newData)
        {
            var result = _datamixing.AddDataMixingScope(newData);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public void DeleteScope([FromBody]DeleteParameterRequestModel model)
        {
            _datamixing.DeleteScope(model);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public void DeleteRule([FromBody]DeleteParameterRequestModel model)
        {
            _datamixing.DeleteRule(model);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public void UpdateScope([FromBody]KeyValueResponseModel model)
        {
            _datamixing.UpdateScope(model);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public void UpdateRule([FromBody]KeyValueResponseModel model)
        {
            _datamixing.UpdateRule(model);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public List<KeyValueResponseModel> GetDataMixingRuleList()
        {
            return _datamixing.GetDataMixingRuleList();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddDataMixingRule([FromBody]AddParameterModel newData)
        {
            var result = _datamixing.AddDataMixingRule(newData);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }
     

        [HttpPost]
        public List<DataMixingMailDocument> GetDocumentMailById(DataMixingIdRequestModel IdDocument)
        {
            return _datamixing.GetDocumentMailById(IdDocument);
        }
        [HttpPost]
        public List<DataMixingExcelDocument> GetDocumentExcelById(DataMixingIdRequestModel IdDocument)
        {
            return _datamixing.GetDocumentExcelById(IdDocument);
        }
        [HttpPost]
        public async Task<ActionResult> AddDataMixingExcelMasterMain([FromForm]IFormFile files_)
        {
            try
            {
                var dmm = new DataMixingExcelDocument();
                var data = 0;
                var dataDoc = (from s in _context.DataMixingExcelDocument



                               select new DataMixingExcelDocument
                               {
                                   DataMixingExcelDocumentId = s.DataMixingExcelDocumentId,
                                   FileName = s.FileName,
                                   ContentType = s.ContentType,
                                   FileSize = s.FileSize,
                                   DataMixingMasterId = s.DataMixingMasterId


                               }).ToList();

                foreach (var i in dataDoc)
                {


                    var item = _context.DataMixingExcelDocument.FirstOrDefault(o => o.DataMixingExcelDocumentId == i.DataMixingExcelDocumentId);
                    if (item == null)
                    {

                        data = 0;
                    }
                    else
                    {
                        data = (from MaxValue in _context.DataMixingExcelDocument
                                select MaxValue.DataMixingExcelDocumentId
                           ).Max();
                    }
                }


                var roothPath = Path.Combine(_environment.ContentRootPath, "excelLinkFile");
                if (!Directory.Exists(roothPath))
                    Directory.CreateDirectory(roothPath);
               

                var filePath = Path.Combine(roothPath, files_.FileName.Replace("ğ", "g").Replace("ı", "i").Replace("ş", "s").Replace("Ğ", "G").Replace("İ", "I").Replace("Ş", "S"));
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    var document = new DataMixingExcelDocument
                    {
                        DataMixingExcelDocumentId = data + 1,
                        FileName = files_.FileName,
                        ContentType = files_.ContentType,
                        FileSize = files_.Length,



                    };
                    await files_.CopyToAsync(stream);
                    _context.DataMixingExcelDocument.Add(document);
                    await _context.SaveChangesAsync();
                }
                return Ok(data + 1);
            }
            catch(Exception e)
            {
                return NotFound(e);

            }
        }
        [HttpPost]
        public async Task<ActionResult> AddDataMixingMailMasterMain([FromForm]IFormFile files_)
        {
            try
            {

                var dmm = new DataMixingMailDocument();
                var data = 0;
                var dataDoc = (from s in _context.DataMixingMailDocument



                               select new DataMixingMailDocument
                               {
                                   DataMixingMailDocumentId = s.DataMixingMailDocumentId,
                                   FileName = s.FileName,
                                   ContentType = s.ContentType,
                                   FileSize = s.FileSize,
                                   DataMixingMasterId = s.DataMixingMasterId


                               }).ToList();

                foreach (var i in dataDoc)
                {


                    var item = _context.DataMixingMailDocument.FirstOrDefault(o => o.DataMixingMailDocumentId == i.DataMixingMailDocumentId);
                    if (item == null)
                    {

                        data = 0;
                    }
                    else
                    {
                        data = (from MaxValue in _context.DataMixingMailDocument
                                select MaxValue.DataMixingMailDocumentId
                           ).Max();
                    }
                }


                var roothPath = Path.Combine(_environment.ContentRootPath, "approvedMailLinkFile");
                if (!Directory.Exists(roothPath))
                    Directory.CreateDirectory(roothPath);


                var filePath = Path.Combine(roothPath, files_.FileName.Replace("ğ", "g").Replace("ı", "i").Replace("ş", "s").Replace("Ğ", "G").Replace("İ", "I").Replace("Ş", "S"));
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    var document = new DataMixingMailDocument
                    {
                        DataMixingMailDocumentId = data + 1,
                        FileName = files_.FileName,
                        ContentType = files_.ContentType,
                        FileSize = files_.Length,



                    };
                    await files_.CopyToAsync(stream);
                    _context.DataMixingMailDocument.Add(document);
                    await _context.SaveChangesAsync();
                }
                return Ok(data + 1);
            }
            catch(Exception e)
            {
                return NotFound(e);
            }
        }
        [HttpPost]
        public async Task<ActionResult> UploadMailFiles([FromForm]IFormFile files_, [FromForm]string version)
        {
            try
            {

                var dmm = new DataMixingMailDocument();
                var data = 0;
                var dataDoc = (from s in _context.DataMixingMailDocument



                               select new DataMixingMailDocument
                               {
                                   DataMixingMailDocumentId = s.DataMixingMailDocumentId,
                                   FileName = s.FileName,
                                   ContentType = s.ContentType,
                                   FileSize = s.FileSize,
                                   DataMixingMasterId = s.DataMixingMasterId


                               }).ToList();

                foreach (var i in dataDoc)
                {


                    var item = _context.DataMixingMailDocument.FirstOrDefault(o => o.DataMixingMailDocumentId == i.DataMixingMailDocumentId);
                    if (item == null)
                    {

                        data = 0;
                    }
                    else
                    {
                        data = (from MaxValue in _context.DataMixingMailDocument
                                select MaxValue.DataMixingMailDocumentId
                           ).Max();
                    }
                }






                var roothPath = Path.Combine(_environment.ContentRootPath, "approvedMailLinkFile");
                if (!Directory.Exists(roothPath))
                    Directory.CreateDirectory(roothPath);


                var filePath = Path.Combine(roothPath, files_.FileName.Replace("ğ", "g").Replace("ı", "i").Replace("ş", "s").Replace("Ğ", "G").Replace("İ", "I").Replace("Ş", "S"));
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    var item = _context.DataMixingMailDocument.FirstOrDefault(o => o.DataMixingMasterId == int.Parse(version));
                    if (item == null)
                    {
                        var document = new DataMixingMailDocument
                        {
                            DataMixingMailDocumentId = data + 1,
                            FileName = files_.FileName,
                            ContentType = files_.ContentType,
                            FileSize = files_.Length,
                            DataMixingMasterId = int.Parse(version)
                        };
                        await files_.CopyToAsync(stream);
                        _context.DataMixingMailDocument.Add(document);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        item.DataMixingMailDocumentId = item.DataMixingMailDocumentId;
                        item.FileName = files_.FileName;
                        item.ContentType = files_.ContentType;
                        item.FileSize = files_.Length;
                        item.DataMixingMasterId = int.Parse(version);
                        await files_.CopyToAsync(stream);
                        _context.DataMixingMailDocument.Update(item);
                        await _context.SaveChangesAsync();
                    }

                    var oldData = _context.DataMixingMaster.FirstOrDefault(x => x.DataMixingMasterId == int.Parse(version));
                    oldData.DataMixingMasterId = oldData.DataMixingMasterId;
                    oldData.DataMixingVersion = oldData.DataMixingVersion;
                    oldData.DataMixingVersionDescription = oldData.DataMixingVersionDescription;
                    oldData.IsApproved = oldData.IsApproved;
                    oldData.FromDate = DateTime.Now;
                    oldData.ExcelLink = oldData.ExcelLink;
                    oldData.ApprovalMailLink = (data + 1);


                    _context.DataMixingMaster.Update(oldData);
                    await _context.SaveChangesAsync();




                }
                return Ok(data + 1);

            }
            catch (Exception e)
            {
                return NotFound(e);
            }


        }

        [HttpPost]
        public async Task<ActionResult> UploadExcelFiles([FromForm]IFormFile files_, [FromForm]string version)
        {
            try
            {

                var dmm = new DataMixingExcelDocument();
                var data = 0;
                var dataDoc = (from s in _context.DataMixingExcelDocument



                               select new DataMixingExcelDocument
                               {
                                   DataMixingExcelDocumentId = s.DataMixingExcelDocumentId,
                                   FileName = s.FileName,
                                   ContentType = s.ContentType,
                                   FileSize = s.FileSize,
                                   DataMixingMasterId = s.DataMixingMasterId


                               }).ToList();

                foreach (var i in dataDoc)
                {


                    var item = _context.DataMixingExcelDocument.FirstOrDefault(o => o.DataMixingExcelDocumentId == i.DataMixingExcelDocumentId);
                    if (item == null)
                    {

                        data = 0;
                    }
                    else
                    {
                        data = (from MaxValue in _context.DataMixingExcelDocument
                                select MaxValue.DataMixingExcelDocumentId
                           ).Max();
                    }
                }






                var roothPath = Path.Combine(_environment.ContentRootPath, "excelLinkFile");
                if (!Directory.Exists(roothPath))
                    Directory.CreateDirectory(roothPath);


                var filePath = Path.Combine(roothPath, files_.FileName.Replace("ğ", "g").Replace("ı", "i").Replace("ş", "s").Replace("Ğ", "G").Replace("İ", "I").Replace("Ş", "S"));
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    var item = _context.DataMixingExcelDocument.FirstOrDefault(o => o.DataMixingMasterId == int.Parse(version));
                    if (item == null)
                    {
                        var document = new DataMixingExcelDocument
                        {
                            DataMixingExcelDocumentId = data + 1,
                            FileName = files_.FileName,
                            ContentType = files_.ContentType,
                            FileSize = files_.Length,
                            DataMixingMasterId = int.Parse(version),
                        };
                        await files_.CopyToAsync(stream);
                        _context.DataMixingExcelDocument.Add(document);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        item.DataMixingExcelDocumentId = item.DataMixingExcelDocumentId;
                        item.FileName = files_.FileName;
                        item.ContentType = files_.ContentType;
                        item.FileSize = files_.Length;
                        item.DataMixingMasterId = int.Parse(version);
                        await files_.CopyToAsync(stream);
                        _context.DataMixingExcelDocument.Update(item);
                        await _context.SaveChangesAsync();
                    }

                    var oldData = _context.DataMixingMaster.FirstOrDefault(x => x.DataMixingMasterId == int.Parse(version));
                    oldData.DataMixingMasterId = oldData.DataMixingMasterId;
                    oldData.DataMixingVersion = oldData.DataMixingVersion;
                    oldData.DataMixingVersionDescription = oldData.DataMixingVersionDescription;
                    oldData.IsApproved = oldData.IsApproved;
                    oldData.FromDate = DateTime.Now;
                    oldData.ExcelLink = (data + 1);
                    oldData.ApprovalMailLink = oldData.ApprovalMailLink;


                    _context.DataMixingMaster.Update(oldData);
                    await _context.SaveChangesAsync();




                }
                return Ok(data + 1);
            }
            catch (Exception e)
            {
                return NotFound(e);
            }



        }
        [HttpPost]

        public int CheckNull(DataMixing master)
        {
            return _datamixing.CheckNull(master);
        }
        [HttpPost]

        public int CheckNullScope(DataMixing master)
        {
            return _datamixing.CheckNullScope(master);
        }
        [HttpPost]
        public List<KeyValueResponseModel> GetByDataMixingRule()
        {
            return _datamixing.GetByDataMixingRule();
        }
        [HttpPost]
        public List<KeyValueResponseModel> GetByDataMixingScope()
        {
            return _datamixing.GetByDataMixingScope();
        }
        [HttpPost]
        public List<KeyValueResponseModel> GetByDataMixingVerison()
        {
            return _datamixing.GetByDataMixingVerison();
        }
        [HttpPost]
        public object GetByDataMixingRuleById(Search<DataMixingRuleRequestModel> request, DataSourceLoadOptionsBase dataSource)
        {
            return _datamixing.GetByDataMixingRuleById(request.Data, dataSource);
        }

        [HttpPost]
        public object GetByDataMixingScopeById(Search<DataMixingScopeRequestModel> request, DataSourceLoadOptionsBase dataSource)
        {
            return _datamixing.GetByDataMixingScopeById(request.Data, dataSource);
        }

   

    }
}




