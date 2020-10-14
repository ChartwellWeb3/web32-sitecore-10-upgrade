using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chartwell.Unit.Tests.Models.NewestProperties
{
  public class NPModel
  {
    public string ItemId { get; set; }
    public string ItemName { get; set; }
    public string ItemPath { get; set; }
    public string TemplateName { get; set; }
    public string LegacyId { get; set; }
    public string PropertyName { get; set; }
    public string PropertyId { get; set; }
    public string USP { get; set; }
    public bool IsOwned { get; set; }
    public bool IsNewProperty { get; set; }
  }
}
