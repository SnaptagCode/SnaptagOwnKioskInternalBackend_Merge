using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace SnaptagOwnKioskInternalBackend.DBContexts.DBModels
{
    public class PurchaseHistoryModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Index { get; set; }
        [Required]
        public int EventIndex { get; set; }
        [Required]
        public int MachineIndex { get; set; }
        [Required]
        public string PhotoAuthNumber { get; set; }
        [Required]
        public int Amount { get; set; } = -1;
        [Required]
        public string AuthSeqNum { get; set; } = "-1";
        [Required]
        public string ApprovalNumber { get; set; } = "-1";
        public DateTime? PurchaseDate { get; set; } = DateTime.Now;
        [Required]
        public bool isRefunded { get; set; } = false;
        [AllowNull]
        public bool isUploaded { get; set; } = false;
        [Required]
        public bool isPrinted { get; set; } = false;
        [Required]
        public string Details { get; set; }


    }
}
