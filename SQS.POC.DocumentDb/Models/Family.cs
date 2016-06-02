using Microsoft.Azure.Documents;

namespace DataDocumentDB.Models
{
    class Family : Resource
    {
        public string FamilyName { get; set; }
        public Parent[] Parents { get; set; }
        public Child[] Children { get; set; }
        public Pet[] Pets { get; set; }
    }
}
