using DataDictionary.Entities;
using hostingDictionary = DataDictionary.Hosting;
using DataDictionary.Utility.Results;
using DataDictionaryWebApi.Configuration;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataDictionary.Models.ResponseModels;

namespace DataDictionaryWebApi.Models
{

    public class DataMixingWorkflow : IDataMixingContract
    {
        private EahouseContext _EahouseContext;
        private IHostingEnvironment _environment;
        public DataMixingWorkflow(EahouseContext context, IHostingEnvironment environment)
        {
            _EahouseContext = context;
            _environment = environment;
        }

        public void Add<T>(T entity) where T : class
        {
            _EahouseContext.Add(entity);

        }
        public bool SaveAll()
        {

            return _EahouseContext.SaveChanges() > 0;

        }



        public List<DataMixingRequestModel> GetDetailDataByVersion(DataMixingMasterIdRequestModel version)
        {
            var dataMixing_new1 = (from s in _EahouseContext.DataMixing
                                   where s.IsActive == true && s.DataMixingScopeId == 0 && s.DataMixingRuleId == 0

                                   select new DataMixingRequestModel
                                   {

                                       DataDictionaryId = s.DataDictionaryId,
                                       DataDescription = s.DataDescription,
                                       DataMixingRule = null,
                                       DataMixingRuleId = s.DataMixingRuleId,
                                       DataMixingDescription = s.DataMixingDescription,
                                       FromDate = s.FromDate,
                                       ThruDate = s.ThruDate,
                                       DataMixingScope = null,
                                       DataMixingScopeId = s.DataMixingScopeId,
                                       DataMixingMasterId = s.DataMixingMasterId,
                                       DataMixingId = s.DataMixingId

                                   }).Where(a => a.DataMixingMasterId == version.DataMixingMasterId).ToList();

            var dataMixing_new2 = (from s in _EahouseContext.DataMixing
                                   join rl in _EahouseContext.DataMixingRule
                                   on s.DataMixingRuleId equals rl.DataMixingRuleId
                                   where s.IsActive == true && s.DataMixingScopeId == 0 && s.DataMixingRuleId != 0

                                   select new DataMixingRequestModel
                                   {

                                       DataDictionaryId = s.DataDictionaryId,
                                       DataDescription = s.DataDescription,
                                       DataMixingRule = rl.DataMixingRuleName,
                                       DataMixingRuleId = s.DataMixingRuleId,
                                       DataMixingDescription = s.DataMixingDescription,
                                       FromDate = s.FromDate,
                                       ThruDate = s.ThruDate,
                                       DataMixingScope = null,
                                       DataMixingScopeId = s.DataMixingScopeId,
                                       DataMixingMasterId = s.DataMixingMasterId,
                                       DataMixingId = s.DataMixingId

                                   }).Where(a => a.DataMixingMasterId == version.DataMixingMasterId).ToList();

            var dataMixing_new3 = (from s in _EahouseContext.DataMixing
                                   join sc in _EahouseContext.DataMixingScope
                                   on s.DataMixingScopeId equals sc.DataMixingScopeId
                                   where s.IsActive == true && s.DataMixingRuleId == 0 && s.DataMixingScopeId != 0

                                   select new DataMixingRequestModel
                                   {

                                       DataDictionaryId = s.DataDictionaryId,
                                       DataDescription = s.DataDescription,
                                       DataMixingRule = null,
                                       DataMixingRuleId = s.DataMixingRuleId,
                                       DataMixingDescription = s.DataMixingDescription,
                                       FromDate = s.FromDate,
                                       ThruDate = s.ThruDate,
                                       DataMixingScope = sc.DataMixingScopeName,
                                       DataMixingScopeId = s.DataMixingScopeId,
                                       DataMixingMasterId = s.DataMixingMasterId,
                                       DataMixingId = s.DataMixingId

                                   }).Where(a => a.DataMixingMasterId == version.DataMixingMasterId).ToList();


            var data = (

                from s in _EahouseContext.DataMixing
                join sc in _EahouseContext.DataMixingScope
                on s.DataMixingScopeId equals sc.DataMixingScopeId
                join rl in _EahouseContext.DataMixingRule
               on s.DataMixingRuleId equals rl.DataMixingRuleId

                where s.IsActive == true && s.DataMixingScopeId != 0 && s.DataMixingRuleId != 0

                select new DataMixingRequestModel
                {
                    DataDictionaryId = s.DataDictionaryId,
                    DataDescription = s.DataDescription,
                    DataMixingRule = rl.DataMixingRuleName,
                    DataMixingRuleId = s.DataMixingRuleId,
                    DataMixingDescription = s.DataMixingDescription,
                    FromDate = s.FromDate,
                    ThruDate = s.ThruDate,
                    DataMixingScope = sc.DataMixingScopeName,
                    DataMixingScopeId = s.DataMixingScopeId,
                    DataMixingMasterId = s.DataMixingMasterId,
                    DataMixingId = s.DataMixingId


                }).Where(a => a.DataMixingMasterId == version.DataMixingMasterId).ToList();
            var allData = data.Concat(dataMixing_new1).Concat(dataMixing_new2).Concat(dataMixing_new3).OrderBy(a => a.DataDictionaryId)
                                    .ToList();
            return allData;
        }



        public List<DataMixingMasterRequestModel> GetDistinct()
        {

            var data = (from s in _EahouseContext.DataMixingMaster
                        join pddMap in _EahouseContext.DataDictionaryStatus
                         on s.IsApproved equals pddMap.ApprovalStatusId
                        where s.IsActive == true

                        select new DataMixingMasterRequestModel
                        {
                            DataMixingMasterId = s.DataMixingMasterId,
                            ApprovalStatusName = pddMap.ApprovalStatusName,
                            DataMixingVersion = s.DataMixingVersion,
                            DataMixingVersionDescription = s.DataMixingVersionDescription,
                            FromDate = s.FromDate,
                            ExcelLink = s.ExcelLink,
                            ApprovalMailLink = s.ApprovalMailLink,
                            IsActive = s.IsActive
                        }).ToList();
            return data;
        }
        public List<DataMixingMailDocument> GetDocumentMailById(DataMixingIdRequestModel IdDocument)
        {


            var data = (from s in _EahouseContext.DataMixingMailDocument



                        select new DataMixingMailDocument
                        {
                            DataMixingMailDocumentId = s.DataMixingMailDocumentId,
                            FileName = s.FileName,
                            ContentType = s.ContentType,
                            FileSize = s.FileSize,
                            DataMixingMasterId = s.DataMixingMasterId
                        }).Where(v => v.DataMixingMasterId == IdDocument.DataMixingMasterId).ToList();
            return data;
        }
        public List<DataMixingExcelDocument> GetDocumentExcelById(DataMixingIdRequestModel IdDocument)
        {


            var data = (from s in _EahouseContext.DataMixingExcelDocument



                        select new DataMixingExcelDocument
                        {
                            DataMixingExcelDocumentId = s.DataMixingExcelDocumentId,
                            FileName = s.FileName,
                            ContentType = s.ContentType,
                            FileSize = s.FileSize,
                            DataMixingMasterId = s.DataMixingMasterId
                        }).Where(v => v.DataMixingMasterId == IdDocument.DataMixingMasterId).ToList();
            return data;
        }


        public List<ApprovalStatusNameRequest> GetApproval()
        {

            var data = (from s in _EahouseContext.DataDictionaryStatus

                        where s.ApprovalStatusId == 5

                        select new ApprovalStatusNameRequest
                        {
                            ApprovalStatusId = s.ApprovalStatusId,
                            ApprovalStatusName = s.ApprovalStatusName
                        }).ToList();
            return data;
        }

        public List<DataMixingMasterRequestModel> GetDistinctbyId(GetByDataMixingMasterIdRequestModel IdDataMixing)
        {

            var data = (from s in _EahouseContext.DataMixingMaster
                        join pddMap in _EahouseContext.DataDictionaryStatus
                         on s.IsApproved equals pddMap.ApprovalStatusId
                        where s.IsActive == true

                        select new DataMixingMasterRequestModel
                        {
                            DataMixingMasterId = s.DataMixingMasterId,
                            ApprovalStatusName = pddMap.ApprovalStatusName,
                            DataMixingVersion = s.DataMixingVersion,
                            DataMixingVersionDescription = s.DataMixingVersionDescription,
                            FromDate = s.FromDate,
                            ExcelLink = s.ExcelLink,
                            ApprovalMailLink = s.ApprovalMailLink,
                            IsActive = s.IsActive
                        }).Where(v => v.DataMixingMasterId == IdDataMixing.Id).ToList();
            return data;
        }

        public List<DataMixingApprovalStatus> GetApprovalStatusbyId(DataMixingApprovalStatus IdDataMixing)
        {

            var data = (from s in _EahouseContext.DataMixingMaster



                        select new DataMixingApprovalStatus
                        {
                            DataMixingMasterId = s.DataMixingMasterId,
                            IsApproved = s.IsApproved,

                        }).Where(v => v.DataMixingMasterId == IdDataMixing.DataMixingMasterId).ToList();
            return data;
        }



        public void UpdateDataMixingMaster([FromBody]DataMixingMasterUpdate newData)
        {

            try
            {
                var role = hostingDictionary.HttpContext.Current.User.Claims.ToList();
                var username = role[0].Value;

                var oldData = _EahouseContext.DataMixingMaster.FirstOrDefault(x => x.DataMixingMasterId == newData.DataMixingMasterId);
                oldData.DataMixingMasterId = oldData.DataMixingMasterId;
                oldData.DataMixingVersion = oldData.DataMixingVersion;
                oldData.DataMixingVersionDescription = newData.DataMixingVersionDescription;
                oldData.IsApproved = oldData.IsApproved;
                oldData.FromDate = oldData.FromDate;
                oldData.ExcelLink = newData.ExcelLink;
                oldData.ApprovalMailLink = newData.ApprovalMailLink;
                oldData.CreatedBy = oldData.CreatedBy;
                oldData.ModifiedBy = username;
                oldData.ModifiedDate = DateTime.Now;


                _EahouseContext.DataMixingMaster.Update(oldData);
                _EahouseContext.SaveChanges();
            }
            catch (Exception)
            {


            }
        }
        public void UpdateDataMixingMasterStatus([FromBody]DataMixingMasterUpdate newData)
        {
            try
            {
                var role = hostingDictionary.HttpContext.Current.User.Claims.ToList();
                var username = role[0].Value;
                var oldData = _EahouseContext.DataMixingMaster.FirstOrDefault(x => x.DataMixingMasterId == newData.DataMixingMasterId);
                oldData.DataMixingMasterId = oldData.DataMixingMasterId;
                oldData.DataMixingVersion = oldData.DataMixingVersion;
                oldData.DataMixingVersionDescription = oldData.DataMixingVersionDescription;
                oldData.IsApproved = newData.IsApproved;
                oldData.FromDate = oldData.FromDate;
                oldData.ExcelLink = oldData.ExcelLink;
                oldData.ApprovalMailLink = oldData.ApprovalMailLink;
                oldData.ModifiedBy = username;
                oldData.ModifiedDate = DateTime.Now;
                oldData.IsActive = true;


                _EahouseContext.DataMixingMaster.Update(oldData);
                _EahouseContext.SaveChanges();
            }
            catch (Exception)
            {


            }
        }

        public IResult AddDataMixingMaster([FromBody]DataMixingMaster newData)
        {
            try
            {
                var role = hostingDictionary.HttpContext.Current.User.Claims.ToList();
                var username = role[0].Value;
                var dmm = new DataMixingMaster();
                var data = 0;
                var dataMaster = (from s in _EahouseContext.DataMixingMaster



                                  select new DataMixingMaster
                                  {
                                      DataMixingMasterId = s.DataMixingMasterId,
                                      DataMixingVersion = s.DataMixingVersion,
                                      DataMixingVersionDescription = s.DataMixingVersionDescription,
                                      IsApproved = s.IsApproved,
                                      FromDate = s.FromDate,
                                      ExcelLink = s.ExcelLink,
                                      ApprovalMailLink = s.ApprovalMailLink,
                                      IsActive = s.IsActive
                                  }).ToList();

                foreach (var i in dataMaster)
                {


                    var item = _EahouseContext.DataMixingMaster.FirstOrDefault(o => o.DataMixingMasterId == i.DataMixingMasterId);
                    if (item == null)
                    {

                        data = 0;
                    }
                    else
                    {
                        data = (from MaxValue in _EahouseContext.DataMixingMaster
                                select MaxValue.DataMixingMasterId
                           ).Max();
                    }
                }
                dmm.DataMixingMasterId = data + 1;
                dmm.DataMixingVersion = newData.DataMixingVersion;
                dmm.DataMixingVersionDescription = newData.DataMixingVersionDescription;
                dmm.IsApproved = newData.IsApproved;
                dmm.FromDate = DateTime.Now;
                dmm.ExcelLink = newData.ExcelLink;
                dmm.ApprovalMailLink = newData.ApprovalMailLink;
                dmm.CreatedBy = username;
                dmm.ModifiedBy = null;
                dmm.ModifiedDate = null;

                dmm.IsActive = true;




                _EahouseContext.DataMixingMaster.Add(dmm);
                _EahouseContext.SaveChanges();


                var dataMailDocument = _EahouseContext.DataMixingMailDocument.FirstOrDefault(x => x.DataMixingMailDocumentId == newData.ApprovalMailLink);
                if (dataMailDocument != null)
                {



                    dataMailDocument.DataMixingMailDocumentId = dataMailDocument.DataMixingMailDocumentId;
                    dataMailDocument.FileName = dataMailDocument.FileName;
                    dataMailDocument.ContentType = dataMailDocument.ContentType;
                    dataMailDocument.FileSize = dataMailDocument.FileSize;
                    dataMailDocument.DataMixingMasterId = (data + 1);
                    _EahouseContext.DataMixingMailDocument.Update(dataMailDocument);
                    _EahouseContext.SaveChanges();


                }

                var dataExcelDocument = _EahouseContext.DataMixingExcelDocument.FirstOrDefault(x => x.DataMixingExcelDocumentId == newData.ExcelLink);
                if (dataExcelDocument != null)
                {



                    dataExcelDocument.DataMixingExcelDocumentId = dataExcelDocument.DataMixingExcelDocumentId;
                    dataExcelDocument.FileName = dataExcelDocument.FileName;
                    dataExcelDocument.ContentType = dataExcelDocument.ContentType;
                    dataExcelDocument.FileSize = dataExcelDocument.FileSize;
                    dataExcelDocument.DataMixingMasterId = (data + 1);
                    _EahouseContext.DataMixingExcelDocument.Update(dataExcelDocument);
                    _EahouseContext.SaveChanges();


                }
                return new SuccessResult("Ekleme işlemi başarılı.");
            }


            catch (Exception exp)
            {

                return new ErrorResult(exp.ToString());
            }
        }


        public void UpdateDataMixingDetail([FromBody]DataMixingDetailUpdateModel newData)
        {
            try
            {
                var role = hostingDictionary.HttpContext.Current.User.Claims.ToList();
                var username = role[0].Value;
                var oldData = _EahouseContext.DataMixing.FirstOrDefault(x => x.DataDictionaryId == newData.DataDictionaryId && x.DataMixingMasterId == newData.DataMixingMasterId);
                oldData.DataDictionaryId = oldData.DataDictionaryId;
                oldData.DataDescription = oldData.DataDescription;
                oldData.DataMixingRuleId = newData.DataMixingRuleId;
                oldData.DataMixingDescription = newData.DataMixingDescription;
                oldData.FromDate = oldData.FromDate;
                oldData.ThruDate = DateTime.Now;
                oldData.DataMixingScopeId = newData.DataMixingScopeId;
                oldData.DataMixingMasterId = oldData.DataMixingMasterId;
                oldData.DataMixingId = oldData.DataMixingId;
                oldData.CreatedBy = oldData.CreatedBy;
                oldData.ModifiedBy = username;
                oldData.ModifiedDate = DateTime.Now;
                _EahouseContext.DataMixing.Update(oldData);
                _EahouseContext.SaveChanges();
            }
            catch (Exception)
            {


            }
        }
        public void DeleteDataMixing([FromBody]DataMixingDictionaryIdRequestModel dataMixingDictionaryIdRequestModel)
        {
            try
            {
                var role = hostingDictionary.HttpContext.Current.User.Claims.ToList();
                var username = role[0].Value;
                var mainData = _EahouseContext.DataMixing.FirstOrDefault(x => x.DataDictionaryId == dataMixingDictionaryIdRequestModel.DataDictionaryId && x.DataMixingMasterId == dataMixingDictionaryIdRequestModel.DataMixingMasterId);
                mainData.DataDictionaryId = mainData.DataDictionaryId; ;
                mainData.DataDescription = mainData.DataDescription;
                mainData.DataMixingRuleId = mainData.DataMixingRuleId;
                mainData.DataMixingDescription = mainData.DataMixingDescription;
                mainData.FromDate = mainData.FromDate;
                mainData.ThruDate = DateTime.Now;
                mainData.CreatedBy = mainData.CreatedBy;
                mainData.ModifiedBy = username;
                mainData.ModifiedDate = DateTime.Now;
                mainData.IsActive = false;
                _EahouseContext.DataMixing.Update(mainData);
                _EahouseContext.SaveChanges();
            }
            catch (Exception)
            {


            }

        }



        public void DeleteDataMixingMaster([FromBody]DataMixingMaster newData)
        {
            try
            {
                var role = hostingDictionary.HttpContext.Current.User.Claims.ToList();
                var username = role[0].Value;
                var mainData = _EahouseContext.DataMixingMaster.FirstOrDefault(x => x.DataMixingMasterId == newData.DataMixingMasterId);
                mainData.DataMixingMasterId = mainData.DataMixingMasterId;
                mainData.DataMixingVersion = mainData.DataMixingVersion;
                mainData.IsApproved = mainData.IsApproved;
                mainData.FromDate = mainData.FromDate;
                mainData.ExcelLink = mainData.ExcelLink;
                mainData.ApprovalMailLink = mainData.ApprovalMailLink;
                mainData.CreatedBy = mainData.CreatedBy;
                mainData.ModifiedBy = username;
                mainData.ModifiedDate = DateTime.Now;
                mainData.IsActive = false;

                _EahouseContext.DataMixingMaster.Update(mainData);
                _EahouseContext.SaveChanges();

                var dataMixingForDelete = (from s in _EahouseContext.DataMixing
                                           where s.DataMixingMasterId == newData.DataMixingMasterId
                                           select new DataMixing
                                           {
                                               DataDictionaryId = s.DataDictionaryId,
                                               DataDescription = s.DataDescription,
                                               DataMixingRuleId = s.DataMixingRuleId,
                                               DataMixingDescription = s.DataMixingDescription,
                                               FromDate = s.FromDate,
                                               ThruDate = s.ThruDate,
                                               DataMixingMasterId = s.DataMixingMasterId,
                                               DataMixingId = s.DataMixingId,
                                               DataMixingScopeId = s.DataMixingScopeId,
                                               IsActive = s.IsActive,

                                           }).AsNoTracking().ToList();//.AsNoTracking was added because of tracking error
                foreach (var j in dataMixingForDelete)
                {
                    var itemj = _EahouseContext.DataMixing.FirstOrDefault(o => o.DataMixingMasterId == j.DataMixingMasterId && o.DataMixingMasterId == newData.DataMixingMasterId && o.DataDictionaryId == j.DataDictionaryId);

                    if (itemj != null)
                    {


                        itemj.DataDictionaryId = itemj.DataDictionaryId;
                        itemj.DataDescription = itemj.DataDescription;
                        itemj.DataMixingRuleId = itemj.DataMixingRuleId;
                        itemj.DataMixingDescription = itemj.DataMixingDescription;
                        itemj.FromDate = itemj.FromDate;
                        itemj.ThruDate = DateTime.Now;
                        itemj.DataMixingMasterId = itemj.DataMixingMasterId;
                        itemj.DataMixingId = itemj.DataMixingId;
                        itemj.DataMixingScopeId = itemj.DataMixingScopeId;
                        itemj.CreatedBy = itemj.CreatedBy;
                        itemj.ModifiedBy = username;
                        itemj.ModifiedDate = DateTime.Now;
                        itemj.IsActive = false;


                        _EahouseContext.DataMixing.Update(itemj);
                        _EahouseContext.SaveChanges();
                    }
                }

                var physicalDataMixingMasterForDelete = (from s in _EahouseContext.PhysicalDataMixingMaster


                                                         where s.DataMixingMasterId == newData.DataMixingMasterId

                                                         select new PhysicalDataMixingMaster
                                                         {
                                                             PhysicalDataMixingMasterId = s.PhysicalDataMixingMasterId,
                                                             PhysicalDataMixingVersion = s.PhysicalDataMixingVersion,
                                                             PhysicalDataMixingVersionDescription = s.PhysicalDataMixingVersionDescription,
                                                             IsApproved = s.IsApproved,
                                                             FromDate = s.FromDate,
                                                             ExcelLink = s.ExcelLink,
                                                             ApprovalMailLink = s.ApprovalMailLink,
                                                             IsActive = s.IsActive,
                                                             CreatedBy = s.CreatedBy,
                                                             ModifiedBy = s.ModifiedBy,
                                                             ModifiedDate = s.ModifiedDate,
                                                             DataMixingMasterId = s.DataMixingMasterId




                                                         }).AsNoTracking().ToList();//.AsNoTracking was added because of tracking error
                foreach (var s in physicalDataMixingMasterForDelete)
                {

                    if (s != null)
                    {



                        s.PhysicalDataMixingMasterId = s.PhysicalDataMixingMasterId;
                        s.PhysicalDataMixingVersion = s.PhysicalDataMixingVersion;
                        s.PhysicalDataMixingVersionDescription = s.PhysicalDataMixingVersionDescription;
                        s.IsApproved = s.IsApproved;
                        s.FromDate = s.FromDate;
                        s.ExcelLink = s.ExcelLink;
                        s.ApprovalMailLink = s.ApprovalMailLink;
                        s.IsActive = false;
                        s.CreatedBy = s.CreatedBy;
                        s.ModifiedBy = username;
                        s.ModifiedDate = DateTime.Now;
                        s.DataMixingMasterId = s.DataMixingMasterId;


                        _EahouseContext.PhysicalDataMixingMaster.Update(s);
                        _EahouseContext.SaveChanges();
                    }
                }




                var physicalDataMixingForDelete = (from s in _EahouseContext.PhysicalDataMixing
                                                   join master in _EahouseContext.PhysicalDataMixingMaster
                                                   on s.PhysicalDataMixingMasterId equals master.PhysicalDataMixingMasterId
                                                   where master.DataMixingMasterId == newData.DataMixingMasterId
                                                   select new PhysicalDataMixing
                                                   {
                                                       DataDictionaryItemId = s.DataDictionaryItemId,
                                                       DDItemServerName = s.DDItemServerName,
                                                       PhysicalDataMixingDescription = s.PhysicalDataMixingDescription,
                                                       DDItemDatabaseName = s.DDItemDatabaseName,
                                                       FromDate = s.FromDate,
                                                       DDItemSchemaName = s.DDItemSchemaName,
                                                       DDItemTableName = s.DDItemTableName,
                                                       DDItemColumnName = s.DDItemColumnName,
                                                       PhysicalDataMixingRuleName = s.PhysicalDataMixingRuleName,
                                                       PhysicalDataMixingId = s.PhysicalDataMixingId,
                                                       ThruDate = s.ThruDate,
                                                       PhysicalDataMixingMasterId = s.PhysicalDataMixingMasterId,
                                                       IsActive = s.IsActive,
                                                       CreatedBy = s.CreatedBy,
                                                       ModifiedBy = s.ModifiedBy,
                                                       ModifiedDate = s.ModifiedDate


                                                   }).AsNoTracking().ToList();//.AsNoTracking was added because of tracking error
                foreach (var j in physicalDataMixingForDelete)
                {

                    if (j != null)
                    {



                        j.DDItemServerName = j.DDItemServerName;
                        j.PhysicalDataMixingDescription = j.PhysicalDataMixingDescription;
                        j.DDItemDatabaseName = j.DDItemDatabaseName;
                        j.FromDate = j.FromDate;
                        j.DDItemSchemaName = j.DDItemSchemaName;
                        j.DDItemTableName = j.DDItemTableName;
                        j.DDItemColumnName = j.DDItemColumnName;
                        j.PhysicalDataMixingRuleName = j.PhysicalDataMixingRuleName;
                        j.PhysicalDataMixingId = j.PhysicalDataMixingId;
                        j.ThruDate = j.ThruDate;
                        j.PhysicalDataMixingMasterId = j.PhysicalDataMixingMasterId;
                        j.IsActive = false;
                        j.CreatedBy = j.CreatedBy;
                        j.ModifiedBy = username;
                        j.ModifiedDate = DateTime.Now;


                        _EahouseContext.PhysicalDataMixing.Update(j);
                        _EahouseContext.SaveChanges();
                    }
                }


            }
            catch (Exception)
            {


            }

        }




        public IResult UpdateDataWithDataDictionary([FromBody]DataMixingMasterIdRequestModel newData)
        {

            try
            {
                var role = hostingDictionary.HttpContext.Current.User.Claims.ToList();
                var username = role[0].Value;
                var data = (from s in _EahouseContext.DataDictionary
                            where (s.DataPrivacyLevelId == 4 || s.DataPrivacyLevelId == 5) && s.CompanyId == 1 && s.HasStructuredData == true

                            select new DataMixingDictionaryRequestModel
                            {
                                DataDictionaryId = s.DataDictionaryId,
                                ApprovalStatusId = s.ApprovalStatusId,
                                DataDescription = s.DataDescription,
                                DataMixingMasterId = newData.DataMixingMasterId,
                                IsActive = s.IsActive

                            }).ToList();

                var count = 0;

                var data_mixing = 0;

                var dataMixing_new = (from s in _EahouseContext.DataMixing

                                      select new DataMixing
                                      {
                                          DataDictionaryId = s.DataDictionaryId,
                                          DataDescription = s.DataDescription,
                                          DataMixingRuleId = s.DataMixingRuleId,
                                          DataMixingDescription = s.DataMixingDescription,
                                          FromDate = s.FromDate,
                                          ThruDate = s.ThruDate,
                                          DataMixingMasterId = s.DataMixingMasterId,
                                          DataMixingId = s.DataMixingId,
                                          DataMixingScopeId = s.DataMixingScopeId,
                                          IsActive = s.IsActive,

                                      }).AsNoTracking().ToList();//.AsNoTracking was added because of tracking error

                var dataMixingForDelete = (from s in _EahouseContext.DataMixing
                                           where s.DataMixingMasterId == newData.DataMixingMasterId
                                           select new DataMixing
                                           {
                                               DataDictionaryId = s.DataDictionaryId,
                                               DataDescription = s.DataDescription,
                                               DataMixingRuleId = s.DataMixingRuleId,
                                               DataMixingDescription = s.DataMixingDescription,
                                               FromDate = s.FromDate,
                                               ThruDate = s.ThruDate,
                                               DataMixingMasterId = s.DataMixingMasterId,
                                               DataMixingId = s.DataMixingId,
                                               DataMixingScopeId = s.DataMixingScopeId,
                                               IsActive = s.IsActive,

                                           }).AsNoTracking().ToList();//.AsNoTracking was added because of tracking error
                var dataMixingDetail_new = (from s in _EahouseContext.DataMixing

                                            where s.DataMixingMasterId == newData.DataMixingMasterId

                                            select new DataMixing
                                            {


                                                DataDictionaryId = s.DataDictionaryId,
                                                DataDescription = s.DataDescription,
                                                DataMixingRuleId = s.DataMixingRuleId,
                                                DataMixingDescription = s.DataMixingDescription,
                                                FromDate = s.FromDate,
                                                ThruDate = s.ThruDate,
                                                DataMixingMasterId = s.DataMixingMasterId,
                                                DataMixingId = s.DataMixingId,
                                                DataMixingScopeId = s.DataMixingScopeId,
                                                IsActive = s.IsActive,
                                            }).AsNoTracking().ToList();

                foreach (var i in dataMixing_new)
                {
                    var item_master = _EahouseContext.DataMixingMaster.FirstOrDefault(o => o.DataMixingMasterId == i.DataMixingMasterId && o.IsActive == true);
                    var item_master2 = _EahouseContext.DataMixingMaster.FirstOrDefault(o => o.DataMixingMasterId == i.DataMixingMasterId && o.IsActive == true && newData.DataMixingMasterId <= i.DataMixingMasterId);
                    if (item_master != null)
                    {
                        if (item_master2 == null)
                        {
                            count = i.DataMixingMasterId;
                        }



                    }

                }
                foreach (var y in dataMixingDetail_new)
                {
                    var itemExistingChecking = _EahouseContext.DataMixing.FirstOrDefault(a => a.DataMixingId == y.DataMixingMasterId);
                    if (itemExistingChecking != null)
                    {
                        _EahouseContext.RemoveRange(_EahouseContext.DataMixing.Where(x => x.DataMixingId == y.DataMixingId).ToList());
                        _EahouseContext.SaveChanges();
                    }
                }


                foreach (var i in data)
                {
                    var dmm = new DataMixing();
                    var item_1 = _EahouseContext.DataMixing.FirstOrDefault(o => o.DataDictionaryId == i.DataDictionaryId && o.DataMixingMasterId == count);
                    var item = _EahouseContext.DataMixing.FirstOrDefault(o => o.DataDictionaryId == i.DataDictionaryId && o.DataMixingMasterId == i.DataMixingMasterId);
                    var item_master_mixing = _EahouseContext.DataMixing.FirstOrDefault(o => o.DataDictionaryId == i.DataDictionaryId && o.DataMixingMasterId == newData.DataMixingMasterId && o.IsActive == true);

                    var dataMixing_new2 = (from s in _EahouseContext.DataMixing

                                           select new DataMixing
                                           {
                                               DataDictionaryId = s.DataDictionaryId,
                                               DataDescription = s.DataDescription,
                                               DataMixingRuleId = s.DataMixingRuleId,
                                               DataMixingDescription = s.DataMixingDescription,
                                               FromDate = s.FromDate,
                                               ThruDate = s.ThruDate,
                                               DataMixingMasterId = s.DataMixingMasterId,
                                               DataMixingId = s.DataMixingId,
                                               DataMixingScopeId = s.DataMixingScopeId,
                                               IsActive = s.IsActive,

                                           }).AsNoTracking().ToList();

                    if (dataMixing_new2.Count == 0)
                    {
                        data_mixing = 0;
                    }
                    else
                    {
                        data_mixing = (from MaxValue in _EahouseContext.DataMixing
                                       select MaxValue.DataMixingId
                          ).Max();

                    }
                    if (item == null)
                    {

                        if (count == 0)
                        {
                            dmm.DataDictionaryId = i.DataDictionaryId;
                            dmm.DataMixingScopeId = 0;
                            dmm.DataMixingRuleId = 0;
                            dmm.DataMixingDescription = null;
                            dmm.DataDescription = i.DataDescription;
                            dmm.DataMixingId = data_mixing + 1;
                            dmm.DataMixingMasterId = newData.DataMixingMasterId;
                            dmm.IsActive = i.IsActive;
                            dmm.FromDate = DateTime.Now;
                            dmm.CreatedBy = username;

                            _EahouseContext.DataMixing.Add(dmm);
                            _EahouseContext.SaveChanges();
                        }
                        else
                        {
                            if (item_1 != null)
                            {
                                if (item_1.IsActive != false)
                                {
                                    dmm.DataDictionaryId = i.DataDictionaryId;
                                    dmm.DataMixingScopeId = item_1.DataMixingScopeId;
                                    dmm.DataMixingRuleId = item_1.DataMixingRuleId;
                                    dmm.DataMixingDescription = item_1.DataMixingDescription;
                                    dmm.DataDescription = i.DataDescription;
                                    dmm.DataMixingId = data_mixing + 1;
                                    dmm.DataMixingMasterId = newData.DataMixingMasterId;
                                    dmm.IsActive = i.IsActive;
                                    dmm.FromDate = DateTime.Now;
                                    dmm.CreatedBy = username;
                                    _EahouseContext.DataMixing.Add(dmm);
                                    _EahouseContext.SaveChanges();
                                }
                                else
                                {
                                    dmm.DataDictionaryId = i.DataDictionaryId;
                                    dmm.DataMixingScopeId = 0;
                                    dmm.DataMixingRuleId = 0;
                                    dmm.DataMixingDescription = null;
                                    dmm.DataDescription = i.DataDescription;
                                    dmm.DataMixingId = data_mixing + 1;
                                    dmm.DataMixingMasterId = newData.DataMixingMasterId;
                                    dmm.IsActive = i.IsActive;
                                    dmm.FromDate = DateTime.Now;
                                    dmm.CreatedBy = username;
                                    _EahouseContext.DataMixing.Add(dmm);
                                    _EahouseContext.SaveChanges();

                                }
                            }

                            else
                            {
                                dmm.DataDictionaryId = i.DataDictionaryId;
                                dmm.DataMixingScopeId = 0;
                                dmm.DataMixingRuleId = 0;
                                dmm.DataMixingDescription = null;
                                dmm.DataDescription = i.DataDescription;
                                dmm.DataMixingId = data_mixing + 1;
                                dmm.DataMixingMasterId = newData.DataMixingMasterId;
                                dmm.IsActive = i.IsActive;
                                dmm.FromDate = DateTime.Now;
                                dmm.CreatedBy = username;
                                _EahouseContext.DataMixing.Add(dmm);
                                _EahouseContext.SaveChanges();


                            }
                        }

                    }


                }
                foreach (var j in dataMixingForDelete)
                {
                    var itemj = _EahouseContext.DataDictionary.FirstOrDefault(o => o.DataDictionaryId == j.DataDictionaryId && j.DataMixingMasterId == newData.DataMixingMasterId && ((o.DataPrivacyLevelId == 4 || o.DataPrivacyLevelId == 5) && o.CompanyId == 1 && o.HasStructuredData == true));
                    var item_j = _EahouseContext.DataDictionary.FirstOrDefault(o => o.DataDictionaryId == j.DataDictionaryId && j.DataMixingMasterId == newData.DataMixingMasterId && ((o.DataPrivacyLevelId == 4 || o.DataPrivacyLevelId == 5) && o.CompanyId == 1 && o.HasStructuredData == true) && o.IsActive == true);

                    if (itemj == null)
                    {
                        j.DataDictionaryId = j.DataDictionaryId;
                        j.DataMixingScopeId = j.DataMixingScopeId;
                        j.DataMixingRuleId = j.DataMixingRuleId;
                        j.DataMixingDescription = j.DataMixingDescription;
                        j.DataMixingId = j.DataMixingId;
                        j.DataMixingMasterId = j.DataMixingMasterId;
                        j.CreatedBy = j.CreatedBy;
                        j.IsActive = false;
                        j.FromDate = j.FromDate;
                        _EahouseContext.DataMixing.Update(j);
                        _EahouseContext.SaveChanges();
                    }


                }

                return new SuccessResult("Ekleme işlemi başarılı.");
            }


            catch (Exception exp)
            {

                return new ErrorResult(exp.ToString());
            }


        }


        public List<KeyValueResponseModel> GetDataMixingScopeList()
        {

            var result = (from k in _EahouseContext.DataMixingScope
                          orderby k.DataMixingScopeId
                          where k.IsActive == true

                          select new KeyValueResponseModel
                          {
                              Key = k.DataMixingScopeId,
                              Value = k.DataMixingScopeName
                          });

            return result.ToList();
        }


        public List<KeyValueResponseModel> GetDataMixingRuleList()
        {

            var result = (from k in _EahouseContext.DataMixingRule
                          orderby k.DataMixingRuleId
                          where k.IsActive == true
                          select new KeyValueResponseModel
                          {
                              Key = k.DataMixingRuleId,
                              Value = k.DataMixingRuleName
                          });

            return result.ToList();
        }

        public IResult AddDataMixingScope([FromBody]AddParameterModel newData)
        {
            try
            {
                var search = _EahouseContext.DataMixingScope.FirstOrDefault(c => c.DataMixingScopeName == newData.Name);
                if (search != null)
                {
                    search.IsActive = true;
                    _EahouseContext.Update(search);
                    _EahouseContext.SaveChanges();
                }
                else
                {
                    var dmm = new DataMixingScope();
                    var data = 0;
                    var dataScope = (from s in _EahouseContext.DataMixingScope



                                     select new DataMixingScope
                                     {
                                         DataMixingScopeId = s.DataMixingScopeId,
                                         DataMixingScopeName = s.DataMixingScopeName,
                                         IsActive = s.IsActive

                                     }).ToList();

                    foreach (var i in dataScope)
                    {


                        var item = _EahouseContext.DataMixingScope.FirstOrDefault(o => o.DataMixingScopeId == i.DataMixingScopeId);
                        if (item == null)
                        {

                            data = 0;
                        }
                        else
                        {
                            data = (from MaxValue in _EahouseContext.DataMixingScope
                                    select MaxValue.DataMixingScopeId
                               ).Max();
                        }
                    }
                    dmm.DataMixingScopeId = data + 1;
                    dmm.DataMixingScopeName = newData.Name;
                    dmm.IsActive = true;





                    _EahouseContext.DataMixingScope.Add(dmm);
                    _EahouseContext.SaveChanges();
                }
                    return new SuccessResult("Ekleme işlemi başarılı.");
                
            }


            catch (Exception exp)
            {

                return new ErrorResult(exp.ToString());
            }
        }


        public IResult AddDataMixingRule([FromBody]AddParameterModel newData)
        {
            try
            {
                var search = _EahouseContext.DataMixingRule.FirstOrDefault(c => c.DataMixingRuleName == newData.Name);
                if (search != null)
                {
                    search.IsActive = true;
                    _EahouseContext.Update(search);
                    _EahouseContext.SaveChanges();
                }
                else
                {
                    var dmm = new DataMixingRule();
                    var data = 0;
                    var dataRule = (from s in _EahouseContext.DataMixingRule



                                    select new DataMixingRule
                                    {
                                        DataMixingRuleId = s.DataMixingRuleId,
                                        DataMixingRuleName = s.DataMixingRuleName,
                                        IsActive = s.IsActive


                                    }).ToList();

                    foreach (var i in dataRule)
                    {


                        var item = _EahouseContext.DataMixingRule.FirstOrDefault(o => o.DataMixingRuleId == i.DataMixingRuleId);
                        if (item == null)
                        {

                            data = 0;
                        }
                        else
                        {
                            data = (from MaxValue in _EahouseContext.DataMixingRule
                                    select MaxValue.DataMixingRuleId
                               ).Max();
                        }
                    }
                    dmm.DataMixingRuleId = data + 1;
                    dmm.DataMixingRuleName = newData.Name;
                    dmm.IsActive = true;





                    _EahouseContext.DataMixingRule.Add(dmm);
                    _EahouseContext.SaveChanges();
                }
                return new SuccessResult("Ekleme işlemi başarılı.");
            }


            catch (Exception exp)
            {

                return new ErrorResult(exp.ToString());
            }
        }

        public void DeleteScope([FromBody]DeleteParameterRequestModel model)
        {
            var data = _EahouseContext.DataMixingScope.FirstOrDefault(x => x.DataMixingScopeId == model.Id);
            data.IsActive = false;
            _EahouseContext.DataMixingScope.Update(data);
            _EahouseContext.SaveChanges();
        }

        public void DeleteRule([FromBody]DeleteParameterRequestModel model)
        {
            var data = _EahouseContext.DataMixingRule.FirstOrDefault(x => x.DataMixingRuleId == model.Id);
            data.IsActive = false;
            _EahouseContext.DataMixingRule.Update(data);
            _EahouseContext.SaveChanges();
        }
        public void UpdateScope([FromBody]KeyValueResponseModel model)
        {
            var data = _EahouseContext.DataMixingScope.FirstOrDefault(x => x.DataMixingScopeId == model.Key);
            data.DataMixingScopeName = model.Value;
            data.IsActive = true;
            _EahouseContext.DataMixingScope.Update(data);
            _EahouseContext.SaveChanges();
        }

        public void UpdateRule([FromBody]KeyValueResponseModel model)
        {
            var data = _EahouseContext.DataMixingRule.FirstOrDefault(x => x.DataMixingRuleId == model.Key);
            data.DataMixingRuleName = model.Value;
            data.IsActive = true;
            _EahouseContext.DataMixingRule.Update(data);
            _EahouseContext.SaveChanges();
        }
        public int CheckNull(DataMixing master)
        {
            var val = 0;
            var data = (from s in _EahouseContext.DataMixing
                        where s.DataMixingMasterId == master.DataMixingMasterId && s.IsActive == true
                        select new DataMixing
                        {
                            DataDictionaryId = s.DataDictionaryId,
                            DataDescription = s.DataDescription,
                            DataMixingRuleId = s.DataMixingRuleId,
                            DataMixingDescription = s.DataMixingDescription,
                            FromDate = s.FromDate,
                            ThruDate = s.ThruDate,
                            DataMixingScopeId = s.DataMixingScopeId,
                            DataMixingMasterId = s.DataMixingMasterId,
                            DataMixingId = s.DataMixingId

                        }).ToList();


            foreach (var dx in data)
            {
                var itemMixing = _EahouseContext.DataMixing.AsNoTracking().FirstOrDefault(o => o.DataMixingId == dx.DataMixingId);
                if (itemMixing.DataMixingRuleId == 0)
                {
                    return val = 10;
                }

                else
                {
                    val = 1;
                }
            }
            return val;
        }
        public int CheckNullScope(DataMixing master)
        {
            var val = 0;
            var data = (from s in _EahouseContext.DataMixing
                        where s.DataMixingMasterId == master.DataMixingMasterId && s.IsActive == true
                        select new DataMixing
                        {
                            DataDictionaryId = s.DataDictionaryId,
                            DataDescription = s.DataDescription,
                            DataMixingRuleId = s.DataMixingRuleId,
                            DataMixingDescription = s.DataMixingDescription,
                            FromDate = s.FromDate,
                            ThruDate = s.ThruDate,
                            DataMixingScopeId = s.DataMixingScopeId,
                            DataMixingMasterId = s.DataMixingMasterId,
                            DataMixingId = s.DataMixingId

                        }).ToList();


            foreach (var dx in data)
            {
                var itemMixing = _EahouseContext.DataMixing.AsNoTracking().FirstOrDefault(o => o.DataMixingId == dx.DataMixingId);
                if (itemMixing.DataMixingScopeId == 0)
                {
                    return val = 10;
                }

                else
                {
                    val = 1;
                }
            }
            return val;
        }
        public List<KeyValueResponseModel> GetByDataMixingRule()
        {
            var result = (from DataMixingRule in _EahouseContext.DataMixingRule
                          where DataMixingRule.IsActive == true
                          select new KeyValueResponseModel()
                          {
                              Key = DataMixingRule.DataMixingRuleId,
                              Value = DataMixingRule.DataMixingRuleName
                          }).ToList();

            return result;
        }
        public List<KeyValueResponseModel> GetByDataMixingScope()
        {
            var result = (from DataMixingScope in _EahouseContext.DataMixingScope
                          where DataMixingScope.IsActive == true
                          select new KeyValueResponseModel()
                          {
                              Key = DataMixingScope.DataMixingScopeId,
                              Value = DataMixingScope.DataMixingScopeName
                          }).ToList();

            return result;
        }
        public List<KeyValueResponseModel> GetByDataMixingVerison()
        {
            var result = (from DataMixingMaster in _EahouseContext.DataMixingMaster
                          where DataMixingMaster.IsActive == true
                          select new KeyValueResponseModel()
                          {
                              Key = DataMixingMaster.DataMixingMasterId,
                              Value = DataMixingMaster.DataMixingVersion
                          }).ToList();

            return result;
        }
        public object GetByDataMixingRuleById(DataMixingRuleRequestModel request, DataSourceLoadOptionsBase loadOptions)
        {////
        
                if (request.DataMixingRuleId == 0 || request == null|| request.DataMixingRuleId ==null)
                {
                    
                var dataMixing_new1 = (from s in _EahouseContext.DataMixing
                                       where s.IsActive == true && s.DataMixingScopeId == 0 && s.DataMixingRuleId == 0

                                       select new DataMixingRequestModel
                                       {

                                           DataDictionaryId = s.DataDictionaryId,
                                           DataDescription = s.DataDescription,
                                           DataMixingRule = null,
                                           DataMixingRuleId = s.DataMixingRuleId,
                                           DataMixingDescription = s.DataMixingDescription,
                                           FromDate = s.FromDate,
                                           ThruDate = s.ThruDate,
                                           DataMixingScope = null,
                                           DataMixingScopeId = s.DataMixingScopeId,
                                           DataMixingMasterId = s.DataMixingMasterId,
                                           DataMixingId = s.DataMixingId

                                       }).Where(a => a.DataMixingMasterId == request.DataMixingMasterId).ToList();

                var dataMixing_new2 = (from s in _EahouseContext.DataMixing
                                       join rl in _EahouseContext.DataMixingRule
                                       on s.DataMixingRuleId equals rl.DataMixingRuleId
                                       where s.IsActive == true && s.DataMixingScopeId == 0 && s.DataMixingRuleId != 0

                                       select new DataMixingRequestModel
                                       {

                                           DataDictionaryId = s.DataDictionaryId,
                                           DataDescription = s.DataDescription,
                                           DataMixingRule = rl.DataMixingRuleName,
                                           DataMixingRuleId = s.DataMixingRuleId,
                                           DataMixingDescription = s.DataMixingDescription,
                                           FromDate = s.FromDate,
                                           ThruDate = s.ThruDate,
                                           DataMixingScope = null,
                                           DataMixingScopeId = s.DataMixingScopeId,
                                           DataMixingMasterId = s.DataMixingMasterId,
                                           DataMixingId = s.DataMixingId

                                       }).Where(a => a.DataMixingMasterId == request.DataMixingMasterId).ToList();

                var dataMixing_new3 = (from s in _EahouseContext.DataMixing
                                       join sc in _EahouseContext.DataMixingScope
                                       on s.DataMixingScopeId equals sc.DataMixingScopeId
                                       where s.IsActive == true && s.DataMixingRuleId == 0 && s.DataMixingScopeId != 0

                                       select new DataMixingRequestModel
                                       {

                                           DataDictionaryId = s.DataDictionaryId,
                                           DataDescription = s.DataDescription,
                                           DataMixingRule = null,
                                           DataMixingRuleId = s.DataMixingRuleId,
                                           DataMixingDescription = s.DataMixingDescription,
                                           FromDate = s.FromDate,
                                           ThruDate = s.ThruDate,
                                           DataMixingScope = sc.DataMixingScopeName,
                                           DataMixingScopeId = s.DataMixingScopeId,
                                           DataMixingMasterId = s.DataMixingMasterId,
                                           DataMixingId = s.DataMixingId

                                       }).Where(a => a.DataMixingMasterId == request.DataMixingMasterId).ToList();


                var data = (

                    from s in _EahouseContext.DataMixing
                    join sc in _EahouseContext.DataMixingScope
                    on s.DataMixingScopeId equals sc.DataMixingScopeId
                    join rl in _EahouseContext.DataMixingRule
                   on s.DataMixingRuleId equals rl.DataMixingRuleId

                    where s.IsActive == true && s.DataMixingScopeId != 0 && s.DataMixingRuleId != 0

                    select new DataMixingRequestModel
                    {
                        DataDictionaryId = s.DataDictionaryId,
                        DataDescription = s.DataDescription,
                        DataMixingRule = rl.DataMixingRuleName,
                        DataMixingRuleId = s.DataMixingRuleId,
                        DataMixingDescription = s.DataMixingDescription,
                        FromDate = s.FromDate,
                        ThruDate = s.ThruDate,
                        DataMixingScope = sc.DataMixingScopeName,
                        DataMixingScopeId = s.DataMixingScopeId,
                        DataMixingMasterId = s.DataMixingMasterId,
                        DataMixingId = s.DataMixingId


                    }).Where(a => a.DataMixingMasterId == request.DataMixingMasterId).ToList();
                var allData = data.Concat(dataMixing_new1).Concat(dataMixing_new2).Concat(dataMixing_new3).OrderBy(a => a.DataDictionaryId)
                                        .ToList();
               




                return DataSourceLoader.Load(allData, loadOptions);

                }
                else
                {
                    
                    var dataMixing_new2 = (from s in _EahouseContext.DataMixing
                                           join rl in _EahouseContext.DataMixingRule
                                           on s.DataMixingRuleId equals rl.DataMixingRuleId
                                           where s.IsActive == true && s.DataMixingScopeId == 0 && s.DataMixingRuleId != 0 && s.DataMixingRuleId == request.DataMixingRuleId

                                           select new DataMixingRequestModel
                                           {

                                               DataDictionaryId = s.DataDictionaryId,
                                               DataDescription = s.DataDescription,
                                               DataMixingRule = rl.DataMixingRuleName,
                                               DataMixingRuleId = s.DataMixingRuleId,
                                               DataMixingDescription = s.DataMixingDescription,
                                               FromDate = s.FromDate,
                                               ThruDate = s.ThruDate,
                                               DataMixingScope = null,
                                               DataMixingScopeId = s.DataMixingScopeId,
                                               DataMixingMasterId = s.DataMixingMasterId,
                                               DataMixingId = s.DataMixingId

                                           }).Where(a => a.DataMixingMasterId == request.DataMixingMasterId).ToList();



                    var data = (

                        from s in _EahouseContext.DataMixing
                        join sc in _EahouseContext.DataMixingScope
                        on s.DataMixingScopeId equals sc.DataMixingScopeId
                        join rl in _EahouseContext.DataMixingRule
                       on s.DataMixingRuleId equals rl.DataMixingRuleId

                        where s.IsActive == true && s.DataMixingScopeId != 0 && s.DataMixingRuleId != 0 && s.DataMixingRuleId == request.DataMixingRuleId

                        select new DataMixingRequestModel
                        {
                            DataDictionaryId = s.DataDictionaryId,
                            DataDescription = s.DataDescription,
                            DataMixingRule = rl.DataMixingRuleName,
                            DataMixingRuleId = s.DataMixingRuleId,
                            DataMixingDescription = s.DataMixingDescription,
                            FromDate = s.FromDate,
                            ThruDate = s.ThruDate,
                            DataMixingScope = sc.DataMixingScopeName,
                            DataMixingScopeId = s.DataMixingScopeId,
                            DataMixingMasterId = s.DataMixingMasterId,
                            DataMixingId = s.DataMixingId


                        }).Where(a => a.DataMixingMasterId == request.DataMixingMasterId).ToList();
                    var allData = data.Concat(dataMixing_new2).OrderBy(a => a.DataDictionaryId)
                                            .ToList();





                    return DataSourceLoader.Load(allData, loadOptions);
                }
         
}
        public object GetByDataMixingScopeById(DataMixingScopeRequestModel request, DataSourceLoadOptionsBase loadOptions)
        {////

            if (request.DataMixingScopeId == 0 || request == null || request.DataMixingScopeId == null)
            {
                
                var dataMixing_new1 = (from s in _EahouseContext.DataMixing
                                       where s.IsActive == true && s.DataMixingScopeId == 0 && s.DataMixingRuleId == 0

                                       select new DataMixingRequestModel
                                       {

                                           DataDictionaryId = s.DataDictionaryId,
                                           DataDescription = s.DataDescription,
                                           DataMixingRule = null,
                                           DataMixingRuleId = s.DataMixingRuleId,
                                           DataMixingDescription = s.DataMixingDescription,
                                           FromDate = s.FromDate,
                                           ThruDate = s.ThruDate,
                                           DataMixingScope = null,
                                           DataMixingScopeId = s.DataMixingScopeId,
                                           DataMixingMasterId = s.DataMixingMasterId,
                                           DataMixingId = s.DataMixingId

                                       }).Where(a => a.DataMixingMasterId == request.DataMixingMasterId).ToList();

                var dataMixing_new2 = (from s in _EahouseContext.DataMixing
                                       join rl in _EahouseContext.DataMixingRule
                                       on s.DataMixingRuleId equals rl.DataMixingRuleId
                                       where s.IsActive == true && s.DataMixingScopeId == 0 && s.DataMixingRuleId != 0

                                       select new DataMixingRequestModel
                                       {

                                           DataDictionaryId = s.DataDictionaryId,
                                           DataDescription = s.DataDescription,
                                           DataMixingRule = rl.DataMixingRuleName,
                                           DataMixingRuleId = s.DataMixingRuleId,
                                           DataMixingDescription = s.DataMixingDescription,
                                           FromDate = s.FromDate,
                                           ThruDate = s.ThruDate,
                                           DataMixingScope = null,
                                           DataMixingScopeId = s.DataMixingScopeId,
                                           DataMixingMasterId = s.DataMixingMasterId,
                                           DataMixingId = s.DataMixingId

                                       }).Where(a => a.DataMixingMasterId == request.DataMixingMasterId).ToList();

                var dataMixing_new3 = (from s in _EahouseContext.DataMixing
                                       join sc in _EahouseContext.DataMixingScope
                                       on s.DataMixingScopeId equals sc.DataMixingScopeId
                                       where s.IsActive == true && s.DataMixingRuleId == 0 && s.DataMixingScopeId != 0

                                       select new DataMixingRequestModel
                                       {

                                           DataDictionaryId = s.DataDictionaryId,
                                           DataDescription = s.DataDescription,
                                           DataMixingRule = null,
                                           DataMixingRuleId = s.DataMixingRuleId,
                                           DataMixingDescription = s.DataMixingDescription,
                                           FromDate = s.FromDate,
                                           ThruDate = s.ThruDate,
                                           DataMixingScope = sc.DataMixingScopeName,
                                           DataMixingScopeId = s.DataMixingScopeId,
                                           DataMixingMasterId = s.DataMixingMasterId,
                                           DataMixingId = s.DataMixingId

                                       }).Where(a => a.DataMixingMasterId == request.DataMixingMasterId).ToList();


                var data = (

                    from s in _EahouseContext.DataMixing
                    join sc in _EahouseContext.DataMixingScope
                    on s.DataMixingScopeId equals sc.DataMixingScopeId
                    join rl in _EahouseContext.DataMixingRule
                   on s.DataMixingRuleId equals rl.DataMixingRuleId

                    where s.IsActive == true && s.DataMixingScopeId != 0 && s.DataMixingRuleId != 0

                    select new DataMixingRequestModel
                    {
                        DataDictionaryId = s.DataDictionaryId,
                        DataDescription = s.DataDescription,
                        DataMixingRule = rl.DataMixingRuleName,
                        DataMixingRuleId = s.DataMixingRuleId,
                        DataMixingDescription = s.DataMixingDescription,
                        FromDate = s.FromDate,
                        ThruDate = s.ThruDate,
                        DataMixingScope = sc.DataMixingScopeName,
                        DataMixingScopeId = s.DataMixingScopeId,
                        DataMixingMasterId = s.DataMixingMasterId,
                        DataMixingId = s.DataMixingId


                    }).Where(a => a.DataMixingMasterId == request.DataMixingMasterId).ToList();
                var allData = data.Concat(dataMixing_new1).Concat(dataMixing_new2).Concat(dataMixing_new3).OrderBy(a => a.DataDictionaryId)
                                        .ToList();





                return DataSourceLoader.Load(allData, loadOptions);

            }
            else
            {

                var dataMixing_new2 = (from s in _EahouseContext.DataMixing
                                       join sc in _EahouseContext.DataMixingScope
                                       on s.DataMixingScopeId equals sc.DataMixingScopeId
                                       where s.IsActive == true && s.DataMixingRuleId == 0 && s.DataMixingScopeId != 0 && s.DataMixingScopeId == request.DataMixingScopeId

                                       select new DataMixingRequestModel
                                       {

                                           DataDictionaryId = s.DataDictionaryId,
                                           DataDescription = s.DataDescription,
                                           DataMixingRule = null,
                                           DataMixingRuleId = s.DataMixingRuleId,
                                           DataMixingDescription = s.DataMixingDescription,
                                           FromDate = s.FromDate,
                                           ThruDate = s.ThruDate,
                                           DataMixingScope = sc.DataMixingScopeName,
                                           DataMixingScopeId = s.DataMixingScopeId,
                                           DataMixingMasterId = s.DataMixingMasterId,
                                           DataMixingId = s.DataMixingId

                                       }).Where(a => a.DataMixingMasterId == request.DataMixingMasterId).ToList();



                var data = (

                    from s in _EahouseContext.DataMixing
                    join rule in _EahouseContext.DataMixingRule
                    on s.DataMixingRuleId equals rule.DataMixingRuleId
                    join scope in _EahouseContext.DataMixingScope
                   on s.DataMixingScopeId equals scope.DataMixingScopeId

                    where s.IsActive == true && s.DataMixingRuleId != 0 && s.DataMixingScopeId != 0 && s.DataMixingScopeId == request.DataMixingScopeId

                    select new DataMixingRequestModel
                    {
                        DataDictionaryId = s.DataDictionaryId,
                        DataDescription = s.DataDescription,
                        DataMixingRule = rule.DataMixingRuleName,
                        DataMixingRuleId = s.DataMixingRuleId,
                        DataMixingDescription = s.DataMixingDescription,
                        FromDate = s.FromDate,
                        ThruDate = s.ThruDate,
                        DataMixingScope = scope.DataMixingScopeName,
                        DataMixingScopeId = s.DataMixingScopeId,
                        DataMixingMasterId = s.DataMixingMasterId,
                        DataMixingId = s.DataMixingId


                    }).Where(a => a.DataMixingMasterId == request.DataMixingMasterId).ToList();
                var allData = data.Concat(dataMixing_new2).OrderBy(a => a.DataDictionaryId)
                                        .ToList();





                return DataSourceLoader.Load(allData, loadOptions);
            }

        }
    }





}

/*
workbook.AddWorksheet("Fiziksel Veri Karşılıkları");
            var wsphy = workbook.Worksheet("Fiziksel Veri Karşılıkları");

int row = 1;
int rowphy = 1;*/