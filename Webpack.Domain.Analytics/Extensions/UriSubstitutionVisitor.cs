using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webpack.Domain.Model.Entities;
using Webpack.Domain.Model.Logic;
using System.Web;

namespace Webpack.Domain.Analytics.Extensions
{
    public class UriSubstitutionVisitor: IVisitor<Page>
    {
        private readonly Dictionary<string, string> urlSubstitutionList;
        public UriSubstitutionVisitor(Dictionary<string, string> urlSubstitutionList)
        {
            if (urlSubstitutionList == null)
            {
                throw new ArgumentNullException("urlSubstitutionList");
            }

            this.urlSubstitutionList = urlSubstitutionList;
        }

        public void Visit(Page page)
        {
            if (page.RawPage == null)
	        {
		        return;
	        }
            var former = page.RawPage;
            var sb = new StringBuilder(former.TextData);
            foreach (var uri in urlSubstitutionList)
            {
                sb.Replace(uri.Value, uri.Key);
            }

            var rp = new RawPage(page.RawPage.Url.Uri, sb.ToString(), former.Reference, former.Links, former.AlternativePaths.ToArray()) 
            { 
                ID = former.ID
            };
            page.RawPage = rp;
        }
    }
}
