using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitFactoryInject
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = CompositionRoot.Configure();
            var factory = container.Resolve<IPaymentGatewayFactory>();
            IPaymentGateway gateway = factory.Create(GateWay.SHIFT4);
            gateway.Invoke();
            gateway = factory.Create(GateWay.WELLSFARGO);
            gateway.Invoke();
            gateway = factory.Create(GateWay.PROTOBASE);
            gateway.Invoke();
            Console.ReadLine();
        }
    }

    public interface IPaymentGateway
    {
        void Invoke();
    }

    public class WellsFargoGateway : IPaymentGateway
    {
        public void Invoke()
        {
            Console.WriteLine("Implement Wells Fargo logic");
        }
    }

    public class SHIFT4Gateway : IPaymentGateway
    {
        public void Invoke()
        {
            Console.WriteLine("Implement SHIFT4 logic");
        }
    }

    public class ProtobaseGateway : IPaymentGateway
    {
        public void Invoke()
        {
            Console.WriteLine("Implement Protobase logic");
        }
    }

    internal class NullGateway : IPaymentGateway
    {

        static Lazy<IPaymentGateway> nullObject = new Lazy<IPaymentGateway>(() => new NullGateway());

        public static IPaymentGateway Empty
        {
            get { return nullObject.Value; }
        }

        private NullGateway()
        {

        }

        public void Invoke()
        {
            //No-op
        }
    }


    public enum GateWay
    {
        PROTOBASE = 1,
        SHIFT4 = 2,
        WELLSFARGO = 3
    }

    public interface IPaymentGatewayFactory
    {
        IPaymentGateway Create(GateWay GateWayType);
    }

    public class DefaultPaymentGatewayFactory : IPaymentGatewayFactory
    {

        readonly Func<GateWay, IPaymentGateway> factoryFactory;

        public DefaultPaymentGatewayFactory(Func<GateWay, IPaymentGateway> factoryFactory)
        {
            this.factoryFactory = factoryFactory;
        }

        public IPaymentGateway Create(GateWay GateWayType)
        {
            return factoryFactory(GateWayType);
        }
    }

    public static class CompositionRoot
    {
        public static IUnityContainer Configure()
        {
            //Dependency injection using unity container
            var container = new UnityContainer();
            //Register the gateways using named mappings
            container.RegisterType<IPaymentGateway, WellsFargoGateway>(GateWay.WELLSFARGO.ToString());
            container.RegisterType<IPaymentGateway, SHIFT4Gateway>(GateWay.SHIFT4.ToString());
            container.RegisterType<IPaymentGateway, ProtobaseGateway>(GateWay.PROTOBASE.ToString());
            //create the strategy
            Func<GateWay, IPaymentGateway> factoryFactory = (gatewayType) =>
                container.Resolve<IPaymentGateway>(gatewayType.ToString()) ?? NullGateway.Empty;
            //register factory
            var factory = new DefaultPaymentGatewayFactory(factoryFactory);
            container.RegisterInstance<IPaymentGatewayFactory>(factory);

            return container;
        }

    }
}
