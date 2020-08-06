Procedimento padrão para implentar nos servidores. 
O motivo dessa configuração é pelo fato de quando reniciar o serviço, no caso dessa aplicação de jobs os mesmos deixam de ser executados


1 - Fazer backup do arquivo C:\Windows\System32\inetsrv\config\applicationHost.config

2 - Copiar o arquivo C:\Windows\System32\inetsrv\config\applicationHost.config para o Desktop

3 - Alterar o arquivo applicationHost.config localizacao no Desktop de acordo com as configurações a baixo

4 - Depois de alterado substituir o arquivo C:\Windows\System32\inetsrv\config\applicationHost.config pelo arquivo applicationHost.config alterado no desktop

5 - Feito este procedimento o serviço já deverá ser iniciado automaticamente

	<system.applicationHost>	
		<serviceAutoStartProviders> 
            <add name="WebJobsAutoStartProvider" type="ECC.WebJobScheduler.StartupService, ECC.WebJobScheduler" /> 
		</serviceAutoStartProviders>
		
        <applicationPools>
			<add name="WebJob" autoStart="true" managedRuntimeVersion="v4.0" startMode="AlwaysRunning">
		</applicationPools>

		<sites>
			<site name="homwebjobs.economizaja.com.br" id="6" serverAutoStart="true">
                <application path="/" applicationPool="WebJob" preloadEnabled="true" serviceAutoStartEnabled="true" serviceAutoStartProvider="WebJobsAutoStartProvider">>
                    <virtualDirectory path="/" physicalPath="E:\Apps\homwebjobs.economizaja.com.br" />
                </application>
                <bindings>
                    <binding protocol="http" bindingInformation="*:80:" />
                </bindings>
            </site>
		</sites>

	</system.applicationHost>