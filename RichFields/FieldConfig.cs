using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;

namespace RichFields
{
    /// <summary>
    /// Utility class used to construct a rich field.
    /// 
    /// Basic pattern is:
    /// 
    /// A wrapper DIV that contains the other controls
    ///     One or more visible controls the user interacts with
    ///     A hidden input that contains the original value from the server
    ///     A hidden input that contains a scrubbed version of the user's changes
    ///       (initially equal to the original value from the server)
    ///       
    /// The user visible controls have javascript attached that scrubs the user
    /// values and stores them in the hidden scrubbed input, and marks the user
    /// controls as "dirty".
    /// 
    /// </summary>
    public class FieldConfig<TModel, TValue>
    {
        public ModelMetadata Metadata { get; set; }
        public RouteValueDictionary RouteValues { get; set; }

        public string InputType { get; set; }
        public string InputClass { get; set; }
        public string ScrubType { get; set; }

        public TValue Value { get; set; }
        public string InitialDisplayValue { get; set; }
        public string InitialScrubbedValue { get; set; }

        public string Id { get; set; }
        public string OrigId { get; set; }
        public string ScrubId { get; set; }

        /// <summary>
        /// The outer-most wrapper tag that contains all the inputs
        /// that make up the RichField
        /// </summary>
        public TagBuilder Tag { get; set; }

        /// <summary>
        /// A list of child tags which includes any user inputs / selects,
        /// A hidden field for the original value, and a hidden field for the
        /// scrubbed value.
        /// </summary>
        public List<TagBuilder> ChildTags { get; set; }

        /// <summary>
        /// Constructor for a FieldConfig
        /// </summary>
        /// <param name="helper">The HTML helper -- contains the view data</param>
        /// <param name="expression"> The value expression </param>
        /// <param name="htmlAttributes">
        ///     Anonymous object with optional HTML attributes.
        ///     These can be used to specify the root ID of the composite control.
        ///     Will also be merged in to the attributes of the main user input
        ///     control(s).
        /// </param>
        public FieldConfig(
            HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression,
            object htmlAttributes)
        {

            Metadata = ModelMetadata.FromLambdaExpression(expression,
                helper.ViewData);
            Value = (TValue)Metadata.Model;
            InitialScrubbedValue = Value == null ? "" : Value.ToString();

            RouteValues = new RouteValueDictionary();
            if (htmlAttributes != null)
            {
                RouteValues = HtmlHelper.AnonymousObjectToHtmlAttributes(
                    htmlAttributes);
            }
            if (!RouteValues.ContainsKey("id"))
            {
                string id = $"ID-{Guid.NewGuid().ToString().ToUpper()}";
                RouteValues["id"] = id;
            }

            Id = RouteValues["id"] as string;
            OrigId = Id + "-ORIG";
            ScrubId = Id + "-SCRUB";

            Tag = MakeWrapperTag();
            ChildTags = new List<TagBuilder>();
        }

        /// <summary>
        /// Create the outermost wrapper div that will contain all the other
        /// tags.
        /// </summary>
        /// <returns></returns>
        private TagBuilder MakeWrapperTag()
        {
            TagBuilder tag = new TagBuilder("div");
            tag.Attributes.Add("class", "input-group");

            return tag;
        }

        /// <summary>
        /// Create a basic input tag for user input.
        /// Some specialized RichFields won't use this if multiple controls
        /// are needed for user input.
        /// </summary>
        /// <returns></returns>
        public TagBuilder InputTag()
        {
            string changeScript =
                $"RichFields.Scrub('{ScrubType}', '{Id}', '{ScrubId}'); " +
                $"RichFields.MarkIfDirty('{Id}', '{OrigId}', '{Id}');";

            TagBuilder tag = new TagBuilder("input");
            tag.Attributes.Add("id", Id);
            tag.Attributes.Add("type", InputType);
            tag.Attributes.Add("class", $"form-control {InputClass}");
            tag.Attributes.Add("autocomplete", "off");
            tag.Attributes.Add("data-lpignore", "true");
            tag.Attributes.Add("onfocus", "this.select();");
            tag.Attributes.Add("onkeyup", changeScript);
            tag.Attributes.Add("onchange", changeScript);
            tag.Attributes.Add("value", InitialDisplayValue);
            tag.MergeAttributes(RouteValues);

            ChildTags.Add(tag);

            return tag;
        }

        /// <summary>
        /// Construct a month selection tag.  This is used instead of the basic
        /// "InputTag" for the MonthYear rich field.
        /// </summary>
        /// <param name="InitialValue"></param>
        /// <returns>the constructed select tag</returns>
        public TagBuilder MonthSelectTag(DateTime? InitialValue)
        {
            var monthFieldId = "MONTH-" + Id;

            string changeScript =
                $"RichFields.Scrub('monthyear-month','{monthFieldId}','{ScrubId}'); "
                + $"RichFields.MarkIfDirty('{monthFieldId}','{OrigId}','{ScrubId}');";

            TagBuilder tag = new TagBuilder("select");
            tag.Attributes.Add("id", monthFieldId);
            tag.Attributes.Add("class", $"form-control {InputClass}");
            tag.Attributes.Add("data-lpignore", "true");
            tag.Attributes.Add("onchange", changeScript);
            tag.Attributes.Add("value", InitialValue.ToString());
            tag.MergeAttributes(RouteValues);

            string selectedValue = InitialValue.HasValue ?
                InitialValue.Value.Month.ToString() : " ";

            var monthItems = new List<(string, string)>
            {
                ( " ", " "),
                ( "1", "Jan"),
                ( "2", "Feb"),
                ( "3", "Mar"),
                ( "4", "Apr"),
                ( "5", "May"),
                ( "6", "Jun"),
                ( "7", "Jul"),
                ( "8", "Aug"),
                ( "9", "Sep"),
                ("10", "Oct"),
                ("11", "Nov"),
                ("12", "Dec")
            };

            foreach (var item in monthItems)
            {
                tag.InnerHtml += MonthOptionHtml(item.Item1, item.Item2,
                    selectedValue);
            }

            ChildTags.Add(tag);

            return tag;
        }

        private string MonthOptionHtml(string value, string name,
            string selectedValue)
        {
            TagBuilder tag = new TagBuilder("option");
            tag.Attributes.Add("value", value);

            if (value == selectedValue) tag.Attributes.Add("selected", "selected");
            tag.InnerHtml = name;

            return tag.ToString();
        }

        /// <summary>
        /// Construct a year selection tag.  This is used instead of the basic
        /// "InputTag" for the MonthYear rich field.
        /// </summary>
        /// <param name="InitialValue"></param>
        /// <returns>the constructed select tag</returns>
        public TagBuilder YearSelectTag(DateTime? InitialValue)
        {
            var yearFieldId = "YEAR-" + Id;

            string changeScript =
                $"RichFields.Scrub('monthyear-year','{yearFieldId}','{ScrubId}'); " +
                $"RichFields.MarkIfDirty('{yearFieldId}','{OrigId}','{ScrubId}');";

            TagBuilder tag = new TagBuilder("input");
            string selectedValue = InitialValue.HasValue ?
                InitialValue.Value.Year.ToString() : " ";

            tag.Attributes.Add("id", yearFieldId);
            tag.Attributes.Add("type", "number");
            tag.Attributes.Add("class", "form-control ml-0 rounded-right");
            tag.Attributes.Add("autocomplete", "off");
            tag.Attributes.Add("data-lpignore", "true");
            tag.Attributes.Add("placeholder", "YYYY");
            tag.Attributes.Add("value", selectedValue);
            tag.Attributes.Add("onfocus", "this.select();");
            tag.Attributes.Add("onchange", changeScript);


            ChildTags.Add(tag);
            return tag;
        }

        /// <summary>
        /// Create the hidden original value tag.  This is used to determine
        /// if the user has edited the value from the original server-saved value.
        /// </summary>
        /// <returns>the constructed tag</returns>
        public TagBuilder OriginalTag()
        {
            TagBuilder tag = new TagBuilder("input");
            tag.Attributes.Add("type", "hidden");
            tag.Attributes.Add("id", OrigId);
            tag.Attributes.Add("value", InitialDisplayValue);

            ChildTags.Add(tag);
            return tag;
        }

        /// <summary>
        /// Create a hidden "scrubbed" input.  Whenever the user changes the value,
        /// a scrubbed version of their value is stored here.  For example, commas and
        /// dollar signs will be removed from numbers during the scrubbing process.
        /// </summary>
        /// <returns>the constructed tag</returns>
        public TagBuilder ScrubbedTag()
        {
            TagBuilder tag = new TagBuilder("input");
            tag.Attributes.Add("type", "hidden");
            tag.Attributes.Add("id", ScrubId);
            tag.Attributes.Add("value", InitialScrubbedValue);
            tag.Attributes.Add("name", Metadata.PropertyName);

            ChildTags.Add(tag);
            return tag;
        }

        /// <summary>
        /// Add a div to the front of the field group to display a prefix
        /// (such as a $)
        /// </summary>
        /// <param name="text"></param>
        /// <returns>the created prefix tag</returns>
        public TagBuilder PrependTextTag(string text)
        {
            TagBuilder tag = new TagBuilder("div");
            tag.Attributes.Add("class", "input-group-prepend");
            tag.InnerHtml = $"<div class='input-group-text'>{text}</div>";

            ChildTags.Add(tag);
            return tag;
        }

        /// <summary>
        /// Add a div to the back of the field group to display a suffix
        /// (such as a unit of measure, like bushels, or the % sign)
        /// </summary>
        /// <param name="text"></param>
        /// <returns>the created suffix tag</returns>
        public TagBuilder AppendTextTag(string text)
        {
            TagBuilder tag = new TagBuilder("div");
            tag.Attributes.Add("class", "input-group-append");
            tag.InnerHtml =
                $"<div class='input-group-text rounded-right'>{text}</div>";

            ChildTags.Add(tag);
            return tag;
        }

        /// <summary>
        /// Output an MvcHtmlString for the wrapper tag and all it's child tags.
        /// </summary>
        /// <returns></returns>
        public MvcHtmlString GetHtmlString()
        {
            foreach (var tag in ChildTags)
            {
                Tag.InnerHtml += tag.ToString();
            }
            return new MvcHtmlString(Tag.ToString());
        }
    }
}
