namespace Web.Controllers
{
    using System.Web.Mvc;

    /// <summary>
    /// The controller.
    /// </summary>
    /// <seealso cref="System.Web.Mvc.Controller" />
    public class HomeController : Controller
    {
        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>The action result.</returns>
        public ActionResult Index()
        {
            return this.View();
        }

        /// <summary>
        /// Abouts this instance.
        /// </summary>
        /// <returns>The action result.</returns>
        public ActionResult About()
        {
            this.ViewBag.Message = "Your application description page.";

            return this.View();
        }

        /// <summary>
        /// Contacts this instance.
        /// </summary>
        /// <returns>The action result.</returns>
        public ActionResult Contact()
        {
            this.ViewBag.Message = "Your contact page.";

            return this.View();
        }
    }
}