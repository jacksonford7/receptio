﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TransactionEmpty.ServicioAnuncianteProblema {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServicioAnuncianteProblema.IServicioAnuncianteProblema")]
    public interface IServicioAnuncianteProblema {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioAnuncianteProblema/AnunciarProblema", ReplyAction="http://tempuri.org/IServicioAnuncianteProblema/AnunciarProblemaResponse")]
        void AnunciarProblema(int idTransaccionQuiosco);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioAnuncianteProblema/AnunciarProblema", ReplyAction="http://tempuri.org/IServicioAnuncianteProblema/AnunciarProblemaResponse")]
        System.Threading.Tasks.Task AnunciarProblemaAsync(int idTransaccionQuiosco);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioAnuncianteProblema/AnunciarProblemaMobile", ReplyAction="http://tempuri.org/IServicioAnuncianteProblema/AnunciarProblemaMobileResponse")]
        void AnunciarProblemaMobile(long idTosProcess, short idZona);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioAnuncianteProblema/AnunciarProblemaMobile", ReplyAction="http://tempuri.org/IServicioAnuncianteProblema/AnunciarProblemaMobileResponse")]
        System.Threading.Tasks.Task AnunciarProblemaMobileAsync(long idTosProcess, short idZona);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioAnuncianteProblema/AnunciarProblemaGenericoMobile", ReplyAction="http://tempuri.org/IServicioAnuncianteProblema/AnunciarProblemaGenericoMobileResp" +
            "onse")]
        void AnunciarProblemaGenericoMobile(string mensajeError, short idZona);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioAnuncianteProblema/AnunciarProblemaGenericoMobile", ReplyAction="http://tempuri.org/IServicioAnuncianteProblema/AnunciarProblemaGenericoMobileResp" +
            "onse")]
        System.Threading.Tasks.Task AnunciarProblemaGenericoMobileAsync(string mensajeError, short idZona);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioAnuncianteProblema/AnunciarProblemaClienteAppTransact" +
            "ion", ReplyAction="http://tempuri.org/IServicioAnuncianteProblema/AnunciarProblemaClienteAppTransact" +
            "ionResponse")]
        void AnunciarProblemaClienteAppTransaction(int idError, short idZona);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioAnuncianteProblema/AnunciarProblemaClienteAppTransact" +
            "ion", ReplyAction="http://tempuri.org/IServicioAnuncianteProblema/AnunciarProblemaClienteAppTransact" +
            "ionResponse")]
        System.Threading.Tasks.Task AnunciarProblemaClienteAppTransactionAsync(int idError, short idZona);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioAnuncianteProblema/AnunciarProblemaServicioWebTransac" +
            "tion", ReplyAction="http://tempuri.org/IServicioAnuncianteProblema/AnunciarProblemaServicioWebTransac" +
            "tionResponse")]
        void AnunciarProblemaServicioWebTransaction(string error, short idZona, int idAplicacion);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioAnuncianteProblema/AnunciarProblemaServicioWebTransac" +
            "tion", ReplyAction="http://tempuri.org/IServicioAnuncianteProblema/AnunciarProblemaServicioWebTransac" +
            "tionResponse")]
        System.Threading.Tasks.Task AnunciarProblemaServicioWebTransactionAsync(string error, short idZona, int idAplicacion);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IServicioAnuncianteProblemaChannel : TransactionEmpty.ServicioAnuncianteProblema.IServicioAnuncianteProblema, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ServicioAnuncianteProblemaClient : System.ServiceModel.ClientBase<TransactionEmpty.ServicioAnuncianteProblema.IServicioAnuncianteProblema>, TransactionEmpty.ServicioAnuncianteProblema.IServicioAnuncianteProblema {
        
        public ServicioAnuncianteProblemaClient() {
        }
        
        public ServicioAnuncianteProblemaClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ServicioAnuncianteProblemaClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServicioAnuncianteProblemaClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServicioAnuncianteProblemaClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void AnunciarProblema(int idTransaccionQuiosco) {
            base.Channel.AnunciarProblema(idTransaccionQuiosco);
        }
        
        public System.Threading.Tasks.Task AnunciarProblemaAsync(int idTransaccionQuiosco) {
            return base.Channel.AnunciarProblemaAsync(idTransaccionQuiosco);
        }
        
        public void AnunciarProblemaMobile(long idTosProcess, short idZona) {
            base.Channel.AnunciarProblemaMobile(idTosProcess, idZona);
        }
        
        public System.Threading.Tasks.Task AnunciarProblemaMobileAsync(long idTosProcess, short idZona) {
            return base.Channel.AnunciarProblemaMobileAsync(idTosProcess, idZona);
        }
        
        public void AnunciarProblemaGenericoMobile(string mensajeError, short idZona) {
            base.Channel.AnunciarProblemaGenericoMobile(mensajeError, idZona);
        }
        
        public System.Threading.Tasks.Task AnunciarProblemaGenericoMobileAsync(string mensajeError, short idZona) {
            return base.Channel.AnunciarProblemaGenericoMobileAsync(mensajeError, idZona);
        }
        
        public void AnunciarProblemaClienteAppTransaction(int idError, short idZona) {
            base.Channel.AnunciarProblemaClienteAppTransaction(idError, idZona);
        }
        
        public System.Threading.Tasks.Task AnunciarProblemaClienteAppTransactionAsync(int idError, short idZona) {
            return base.Channel.AnunciarProblemaClienteAppTransactionAsync(idError, idZona);
        }
        
        public void AnunciarProblemaServicioWebTransaction(string error, short idZona, int idAplicacion) {
            base.Channel.AnunciarProblemaServicioWebTransaction(error, idZona, idAplicacion);
        }
        
        public System.Threading.Tasks.Task AnunciarProblemaServicioWebTransactionAsync(string error, short idZona, int idAplicacion) {
            return base.Channel.AnunciarProblemaServicioWebTransactionAsync(error, idZona, idAplicacion);
        }
    }
}
