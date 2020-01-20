﻿using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//using Newtonsoft.Json.Linq;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
using Our.Umbraco.OpeningHours.Extensions;
using Our.Umbraco.OpeningHours.Model.Json;
using Umbraco.Core;

namespace Our.Umbraco.OpeningHours.Model.Items {
    
    public class OpeningHoursWeekdayItem : OpeningHoursJsonObject {

        #region Properties

        public DayOfWeek DayOfWeek { get; protected set; }

        /// <summary>
        /// Gets the label of the day.
        /// </summary>
        [JsonProperty("label", Order = 1)]
        public string Label { get; private set; }

        /// <summary>
        /// Gets an array of the time slots of the day.
        /// </summary>
        [JsonProperty("items", Order = 3)]
        public OpeningHoursTimeSlot[] Items { get; private set; }

        /// <summary>
        /// Gets where the entity has at least one open time slot throughout the day.
        /// </summary>
        [JsonIgnore]
        public bool IsOpen {
            get { return Items != null && Items.Length > 0; }
        }

        /// <summary>
        /// Gets whether the entity is closed throughout the entire day.
        /// </summary>
        [JsonIgnore]
        public bool IsClosed {
            get { return !IsOpen; }
        }

        /// <summary>
        /// Gets whether the entity is during multiple periods throughout the day.
        /// </summary>
        [JsonIgnore]
        public bool HasMultiple {
            get { return Items != null && Items.Length > 1; }
        }

        /// <summary>
        /// Gets the name of the weekday according to <see cref="CultureInfo.CurrentCulture"/>.
        /// </summary>
        [JsonIgnore]
        public virtual string WeekDayName {
            get { return DateTimeFormatInfo.CurrentInfo == null ? DayOfWeek + "" : DateTimeFormatInfo.CurrentInfo.GetDayName(DayOfWeek); }
        }

        /// <summary>
        /// Gets the name of the weekday according to <see cref="CultureInfo.CurrentCulture"/>. The first character of
        /// the name will always be uppercase.
        /// </summary>
        [JsonIgnore]
        public virtual string WeekDayNameFirstCharToUpper {
            get { return WeekDayName.ToFirstUpper(); }
        }

        #endregion

        #region Constructors

        protected OpeningHoursWeekdayItem(JObject obj, DayOfWeek dayOfWeek) : base(obj) {
            DayOfWeek = dayOfWeek;
            Label = obj.GetString("label") ?? "";
            Items = obj.GetArray("items", OpeningHoursTimeSlot.Parse);
            ParseLegacyValues();
        }

        #endregion

        #region Member methods

        /// <summary>
        /// Parses legacy values from <code>beta1</code>.
        /// </summary>
        private void ParseLegacyValues() {
            if (Items != null) return;
            if (JObject.GetBoolean("isOpen")) {
                TimeSpan opens = JObject.GetString("opens", TimeSpan.Parse);
                TimeSpan closes = JObject.GetString("closes", TimeSpan.Parse);
                Items = new[] {
                    new OpeningHoursTimeSlot(opens, closes)
                };
            } else {
                Items = new OpeningHoursTimeSlot[0];
            }
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Initializes an empty instance for the specified <see cref="DayOfWeek"/>.
        /// </summary>
        /// <param name="dayOfWeek">The day of the week.</param>
        /// <returns>Returns an instance of <see cref="OpeningHoursWeekdayItem"/>.</returns>
        public static OpeningHoursWeekdayItem GetEmptyModel(DayOfWeek dayOfWeek) {
            return new OpeningHoursWeekdayItem(JObject.Parse("{label:\"" + dayOfWeek + "\",items:[]}"), dayOfWeek);
        }

        /// <summary>
        /// Gets an instance of <see cref="OpeningHoursTimeSlot"/> from the specified <see cref="JObject"/>.
        /// </summary>
        /// <param name="obj">The instance of <see cref="JObject"/> to parse.</param>
        /// <param name="dayOfWeek">The day of the week.</param>
        public static OpeningHoursWeekdayItem Parse(JObject obj, DayOfWeek dayOfWeek) {
            return obj == null ? null : new OpeningHoursWeekdayItem(obj, dayOfWeek);
        }

        #endregion

    }

}