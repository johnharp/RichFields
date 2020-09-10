using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace RichFields
{
    public static class Date
    {
        /// <summary>
        /// HTML Helper for Date RichField.
        /// Depends on the jqueryUI Datepicker widget
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
        public static MvcHtmlString rfDateFor<TModel, TValue>(
            this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression,
            object htmlAttributes = null)
        {
            var config = new FieldConfig<TModel, TValue>(helper, expression,
                htmlAttributes);
            string value = config.Value == null ?
                "" :
                string.Format("{0:M/d/yy}", config.Value);

            config.InputType = "text";
            config.InputClass = "rf-date rounded-right apply-datepicker";
            config.ScrubType = "";
            config.InitialDisplayValue = value;
            config.InitialScrubbedValue = value;

            var inputTag = config.InputTag();
            inputTag.Attributes.Add("placeholder", "mm/dd/yy");

            config.OriginalTag();
            config.ScrubbedTag();

            return config.GetHtmlString();
        }

    }
}
