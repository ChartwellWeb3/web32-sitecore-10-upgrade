using System.Web.Mvc;
using Sitecore.EmailCampaign.SampleNewsletter.Controllers;
using Sitecore.EmailCampaign.SampleNewsletter.Models;
using Sitecore.EmailCampaign.SampleNewsletter.Repositories;
using Sitecore.Mvc.Presentation;


public class ExmNewsletterViewsController : Controller
{
  private readonly ViewModelRepository _repository;

  public ExmNewsletterViewsController() : this(new ViewModelRepository())
  {

  }

  public ExmNewsletterViewsController(ViewModelRepository repository)
  {
    this._repository = repository;
  }

  public ActionResult Header()
  {
    return View("~/Views/layouts/SampleNewsletter/Header.cshtml", _repository.GetFixedSectionViewModel(RenderingContext.Current.Rendering));
  }

  public ActionResult ContentSection()
  {
    SectionViewModel sectionViewModel = this._repository.GetSectionViewModel(RenderingContext.Current.Rendering);
    return base.View("~/Views/layouts/SampleNewsletter/ContentSection.cshtml", sectionViewModel);
  }

  public ActionResult Footer()
  {
    SectionViewModel fixedSectionViewModel = this._repository.GetFixedSectionViewModel(RenderingContext.Current.Rendering);
    return base.View("~/Views/layouts/SampleNewsletter/Footer.cshtml", fixedSectionViewModel);
  }

  public ActionResult ImageSection()
  {
    SectionViewModel sectionViewModel = this._repository.GetSectionViewModel(RenderingContext.Current.Rendering);
    return base.View("~/Views/layouts/SampleNewsletter/ImageSection.cshtml", sectionViewModel);
  }

  public ActionResult ListSection()
  {
    ListSectionViewModel listSectionViewModel = this._repository.GetListSectionViewModel(RenderingContext.Current.Rendering);
    return base.View("~/Views/layouts/SampleNewsletter/ListSection.cshtml", listSectionViewModel);
  }

  public ActionResult SingleCtaSection()
  {
    SectionViewModel sectionViewModel = this._repository.GetSectionViewModel(RenderingContext.Current.Rendering);
    return base.View("~/Views/layouts/SampleNewsletter/SingleCTASection.cshtml", sectionViewModel);
  }

  public ActionResult TwoColumnCtaSection()
  {
    ListSectionViewModel listSectionViewModel = this._repository.GetListSectionViewModel(RenderingContext.Current.Rendering);
    return base.View("~/Views/layouts/SampleNewsletter/TwoColumnCTASection.cshtml", listSectionViewModel);
  }
}
