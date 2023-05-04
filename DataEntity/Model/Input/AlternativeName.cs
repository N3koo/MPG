using FluentNHibernate.Mapping;

using SAPServices;
using System;

namespace DataEntity.Model.Input {

    /// <summary>
    /// Class the manipulates the alternative names of the products
    /// </summary>
    public class AlternativeName {
        public virtual string MaterialID { set; get; }
        public virtual string Language { set; get; }
        public virtual string LanguageID { set; get; }
        public virtual string Description { set; get; }
        public virtual string AlternativeDesc { set; get; }
        public virtual string Colour { set; get; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public AlternativeName() {
        }

        /// <summary>
        /// Constructor that copies data from the SAP
        /// </summary>
        /// <param name="data">Object with information from SAP</param>
        public AlternativeName(ZALTERNATIVEDESCRIPTION data) {
            MaterialID = data.MATERIALID;
            Language = data.LANGUAGE;
            LanguageID = data.LANGUAGEID;
            Description = data.DESCRIPTION;
            AlternativeDesc = data.ALTDESCR;
            Colour = data.COLOUR;
        }

        /// <summary>
        /// Used to override the equals function
        /// </summary>
        /// <param name="obj">Reference to the object</param>
        /// <returns></returns>
        public override bool Equals(object obj) {
            if (obj is not AlternativeName other) return false;
            if (ReferenceEquals(this, other)) return true;

            return MaterialID == other.MaterialID && Language == other.Language;
        }

        /// <summary>
        /// Used to override the function
        /// </summary>
        /// <returns>Hash code for the needed fields</returns>
        public override int GetHashCode() {
            return HashCode.Combine(MaterialID, Language);
        }

        /// <summary>
        /// Used to set details
        /// </summary>
        /// <param name="data">Element with the details</param>
        public virtual void SetDetails(ZALTERNATIVEDESCRIPTION data) {
            Language = data.LANGUAGE;
            Description = data.DESCRIPTION;
            AlternativeDesc = data.ALTDESCR;
            Colour = data.COLOUR;
        }

        public virtual void SetDetails(AlternativeName name) {
            Language = name.Language;
            Description = name.Description;
            AlternativeDesc = name.AlternativeDesc;
            Colour = name.Colour;
        }
    }

    /// <summary>
    /// Mapping the alternative name class
    /// </summary>
    public class AlternativeNameMap : ClassMap<AlternativeName> {

        /// <summary>
        /// Constructor
        /// </summary>
        public AlternativeNameMap() {
            Table("MES2MPG_MaterialDataAlternativeDescriptions");

            _ = CompositeId().KeyProperty(x => x.MaterialID).KeyProperty(x => x.Language);

            _ = Map(x => x.LanguageID).Nullable();
            _ = Map(x => x.Description).Nullable();
            _ = Map(x => x.AlternativeDesc).Nullable();
            _ = Map(x => x.Colour).Nullable();
        }
    }
}
