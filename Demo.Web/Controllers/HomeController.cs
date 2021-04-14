using MiniStateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Demo.Web.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            StateMachine.ErrorCode errCode = StateMachine.ErrorCode.NoError;

            Response.Write(StateMachine.GetErrorMessage(errCode));
            Response.Write("<br>");

            StateMachine stateMachine = new StateMachine()
                .AddState(new State("A"))
                .AddState(new State("B"))
                .AddState(new State("C", isEndState: true));

            stateMachine.FindState("A").AddTransition(new Transition("gotoB", stateMachine.FindState("B")));
            //stateMachine.FindState("B").AddTransition(new Transition("gotoC", stateMachine.FindState("C")));

            //State state = stateMachine.States["A"];

            Response.Write(stateMachine.CurrentState);
            Response.Write("<br>");
            
            foreach(Transition transition in stateMachine.GetCurrentTransitions())
            {
                Response.Write(transition);
                Response.Write("<br>");
            }

            //Transition gotoB = stateMachine.FindTransitionFromCurrentState("gotoB");
            stateMachine.ExecuteTransition("gotoB");

            Response.Write(stateMachine.CurrentState);
            Response.Write("<br>");

            stateMachine.GotoState("A");

            return View();
        }
    }
}