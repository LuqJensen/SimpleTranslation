using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Umbraco.Core.Persistence;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.WebApi;

namespace Struct.Umbraco.SimpleTranslation.WebTest.App_Plugins.SimpleTranslation.Controllers.Api
{
    public class PairsController : UmbracoAuthorizedApiController
    {
        [HttpGet]
        public object GetTranslatableKeys()
        {
            var db = DatabaseContext.Database;
            var results = db.Fetch<dynamic>(new Sql().Select("*").From("dbo.cmsDictionary"));
            var subNodes = results.Where(x => x.parent != null).ToLookup(x => x.parent, x => x);
            var rootNodes = results.Where(x => x.parent == null);
            dynamic result = new
            {
                children = new Dictionary<dynamic, dynamic>()
            };

            foreach (var v in rootNodes)
            {
                result.children.Add(v.id, v);
                BuildDictionary(v, subNodes);
            }
            return result;
        }

        private void BuildDictionary(dynamic currentNode, ILookup<dynamic, dynamic> subNodes)
        {
            foreach (var child in subNodes[currentNode.id])
            {
                // Weird issue with .Count and .Any() on IGrouping<dynamic, dynamic>. So iterate once and break instead.
                currentNode.children = new Dictionary<dynamic, dynamic>();
                foreach (var v in subNodes[currentNode.id])
                {
                    currentNode.children.Add(v.id, v);
                    BuildDictionary(v, subNodes);
                }
                break;
            }
        }
    }
}