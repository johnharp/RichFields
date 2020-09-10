using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;

namespace RichFields
{
    public static class MonthYear
    {
        /// <summary>
        /// HTML Helper for the MonthYear RichField.
        /// User enters data via a select for the month
        /// and an input for the year.  They are combined in to the
        /// scrubbed input for submission to the server.
        /// </summary>
        /// 
        /// <typeparam name="TModel">Type of the model</typeparam>
        /// <typeparam name="TValue">Type of the value</typeparam>
        /// <param name="helper">HTML Helper</param>
        /// <param name="expression">Value expression supplied</param>
        /// <param name="htmlAttributes">
        ///     optional object specifying HTML attributes to append
        /// </param>
        /// <returns>The html to inject in the razor page.</returns>
        public static MvcHtmlString rfEditMonthYearFor<TModel, TValue>(
            this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression,
            object htmlAttributes = null)
        {
            var config = new FieldConfig<TModel, TValue>(helper, expression,
                htmlAttributes);
            
            config.InitialScrubbedValue = config.Value.ToString();
            TagBuilder tag = config.Tag;

            TagBuilder monthTag = config.MonthSelectTag(config.Value as DateTime?);
            tag.InnerHtml += monthTag.ToString();
            tag.InnerHtml += config.YearSelectTag(config.Value as DateTime?);


            var scrubbedTag = config.ScrubbedTag();
            tag.InnerHtml += scrubbedTag.ToString();

            return new MvcHtmlString(tag.ToString());
        }
    }
}
