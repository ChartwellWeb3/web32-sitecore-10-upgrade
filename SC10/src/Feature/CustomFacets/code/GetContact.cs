using Sitecore.Data;
using Sitecore.Framework.Conditions;
using Sitecore.Modules.EmailCampaign.Core.Contacts;
using Sitecore.Modules.EmailCampaign.Core.Pipelines.GetContact;
using System;
using System.Linq;

namespace Chartwell.Feature.CustomFacets
{
  //Extending getContact pipeline for exm preview feature
  public class GetContact
  {
    private readonly IContactService _contactService;

    public GetContact(IContactService contactService)
    {
      Condition.Requires(contactService, nameof(contactService)).IsNotNull();

      _contactService = contactService;
    }

    public void Process(GetContactPipelineArgs args)
    {
      Condition.Requires(args, nameof(args)).IsNotNull();

      if (args.ContactIdentifier == null && ID.IsNullOrEmpty(args.ContactId))
      {
        throw new ArgumentException("Either the contact identifier or the contact id must be set");
      }

      string[] facetKeys = args.FacetKeys.Concat(new[]
      {
                "ContactExt" // the custom facet name
            }).ToArray();

      args.Contact =
          args.ContactIdentifier != null ?
              _contactService.GetContact(args.ContactIdentifier, facetKeys)
              :
              _contactService.GetContact(args.ContactId, facetKeys);
    }
  }
}