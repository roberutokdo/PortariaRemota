Teste para empresa Kiper.

Contexto:
 * Criar aplicativo para controle de portaria remota. O aplicativo dever� conter cadastro completo para Apartamentos e Moradores, al�m de listagens e consultas.

Regras:
 * O aplicativo n�o pode cadastrar um Apartamento sem um devido morador vinculado ao mesmo.
 * O aplicativo dever� ter controle de acesso atrav�s de Login.
 * Somente usu�rio logado no sistema ter� acesso aos diversos conte�dos.
 * As pesquisas devem ser din�micas, ou seja, deve ser poss�vel pesquisar por qualquer campo dos dados de Apartamento/Morador.
 * O aplicativo deve ter o back-end separado do fron-end.

------------------------------------------------------------------------------------------------------------------------------------

Resultado:
Foi criada seguinte solu��o:
1 - PortariaRemotaAPI: Projeto API Core para acesso e gerenciamento dos dados no banco MySql, EF Code First que � respons�vel por criar todo o modelo automaticamente e Fluent Validation que � respons�vel por validar todo o contexto antes de enviar qualquer dado para o banco.
2 - WebPortariaRemota: Projeto Asp.Net Core para exibi��o e manipula��o dos dados disponibilizados pela API.
3 - TestPortariaRemotaApi: Projeto XUnit criado para validar regras da API.

Instala��o:
 * Importante: � necess�ria uma m�quina (Windows, Linux ou MacOS) com framework .NET Core 2.1 ou superior instalado e banco de dados MySql com acesso liberado na porta: 3306. Deve-se criar no banco o usu�rio Kiper com senha Kiper@2018 com total acesso no servidor.
 * Caso n�o seja poss�vel criar o usu�rio solicitado, certificar-se de que o arquivo appsettings.json (projeto PortariaRemotaAPI) est� devidamente configurado com o endere�o, porta, usu�rio e senha do banco MySql dispon�vel para uso.

  Via prompt de comandos:
    1 - Acessar diret�rio PortariaRemotaAPI.
      * - Executar comando: > dotnet ef database update . Este comando � respons�vel por criar a base dados e configur�-la com os dados iniciais.
      * - Executar comando: > dotnet run . Este comando inicia a API, liberando seu acesso para Localhost:5000 (http) e Locahost:5001 (https). Para testar a API e seu funcionamento voc� pode abrir um browser e acessar o endere�o: https://localhost:5001/swagger -- O swagger foi usado para gera��o autom�tica da documenta��o e uso da API.
    2 - Abrir um segundo prompt de comandos e acessar o diret�rio WebPortariaRemota.
      * - Executar o comando: > dotnet run. Este comando inicia a API, liberando seu acesso para Localhost:5002 (http) e Locahost:5003 (https). Para acessar o site basta usar o endere�o: https://localhost:5003


   O web site foi criado com as seguintes p�ginas:

   Home -> Usada apenas como p�gina de apresenta��o.
   Apartamentos -> Usada para gerenciamento dos apartamentos, nesta p�gina est� dispon�vel o acesso � visualiza��o, inser��o, altera��o e exclus�o de registros.
   Moradores -> Usada para gerenciamento dos moradores, nesta p�gina est� dispon�vel o acesso � visualiza��o, inser��o, altera��o e exclus�o de registros.
   * Todas as p�ginas, exceto Home, exigem que o usu�rio esteja logado no sistema, para isso foi criado o login:
     User : Kiper
     Senha: Kiper@2018
   * N�o foi disponibilizada uma p�gina para cadastro de novos usu�rios, levando em considera��o que isto tornaria mais complexa a implementa��o, visto que usu�rios possuem diferentes perfis e n�o foi definido como seria estes perfis em rela��o ao acesso dos cadastros, listas, etc.


    Os Testes:
    Foi criado um projeto de Testes usando o XUnit. Para executar o projeto use os passos abaixo:
    1 - Abra um novo prompt de comando e acesse o diret�rio TestPortariaRemotaApi\bin\Debug\netcoreapp2.1.
      * - Executar o comando: > dotnet vstest TestPortariaRemotaApi.dll -lt
          Este comando � respons�vel por listar todos os testes dispon�veis no projeto. Foi criado um total de 20 testes para o projeto da API.
      * - Executar o comando: > dotnet vstest TestPortariaRemotaApi.dll
          Este comando executa todos os testes e ao final � listado apenas o resultado de quantos testes deram OK e quantos falharam.