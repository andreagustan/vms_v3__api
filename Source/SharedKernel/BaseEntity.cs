using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VMS.SharedKernel
{
    public abstract class BaseEntity
    {
        [Column(Order = 0)]
        public System.Int64 Id { get; set; }

        public List<BaseDomainEvent> Events = new List<BaseDomainEvent>();

        public DateTime CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        [Column("CreateUser")]
        [Display(Name = "Creator")]
        public string CreateUser { get; set; }

        [Column("UpdateUser")]
        [Display(Name = "Modifier")]
        public string UpdateUser { get; set; }

        public string CreatedByWithUserNameOnly { get { if (this.CreateUser != null) { if (this.CreateUser.Contains("|")) { return this.CreateUser.Split("|")[0]; } else { return this.CreateUser; } } else { return "N/A"; } } }
        public string CreatedAtFormated { get { return this.CreateDate.ToString("dd MMM yyyy HH:mm:ss"); } }

        public string ModifiedByWithUserNameOnly { get { if (this.UpdateUser != null) { if (this.UpdateUser.Contains("|")) { return this.UpdateUser.Split("|")[0]; } else { return this.UpdateUser; } } else { return "N/A"; } } }
        public string ModifiedAtFormated { get { return this.UpdateDate.HasValue ? this.UpdateDate.Value.ToString("dd MMM yyyy HH:mm:ss") : "N/A"; } }

        public string Creator { get; set; }
        public string Updater { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string DeletedUser { get; set; }

        public string DeletedByWithUserNameOnly { get { if (this.DeletedUser != null) { if (this.DeletedUser.Contains("|")) { return this.DeletedUser.Split("|")[0]; } else { return this.DeletedUser; } } else { return "N/A"; } } }

        public string DeletedAtFormated { get { return this.DeletedDate.HasValue ? this.DeletedDate.Value.ToString("dd MMM yyyy HH:mm:ss") : "N/A"; } }


    }
}
