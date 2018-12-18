# PortariaRemota

Teste para empresa Kiper.

Contexto:
 * Criar aplicativo para controle de portaria remota. O aplicativo deverá conter cadastro completo para Apartamentos e Moradores, além de listagens e consultas.

Regras:
 * O aplicativo não pode cadastrar um Apartamento sem um devido morador vinculado ao mesmo.
 * O aplicativo deverá ter controle de acesso através de Login.
 * Somente usuário logado no sistema terá acesso aos diversos conteúdos.
 * As pesquisas devem ser dinâmicas, ou seja, deve ser possível pesquisar por qualquer campo dos dados de Apartamento/Morador.
 * O aplicativo deve ter o back-end separado do fron-end.

------------------------------------------------------------------------------------------------------------------------------------

Resultado:
Foi criada seguinte solução:
1 - PortariaRemotaAPI: Projeto API Core para acesso e gerenciamento dos dados no banco MySql, EF Code First que é responsável por criar todo o modelo automaticamente e Fluent Validation que é responsável por validar todo o contexto antes de enviar qualquer dado para o banco.
2 - WebPortariaRemota: Projeto Asp.Net Core para exibição e manipulação dos dados disponibilizados pela API.
3 - TestPortariaRemotaApi: Projeto XUnit criado para validar regras da API.

Instalação:
 * Importante: É necessária uma máquina (Windows, Linux ou MacOS) com framework .NET Core 2.1 ou superior instalado e banco de dados MySql com acesso liberado na porta: 3306. Deve-se criar no banco o usuário Kiper com senha Kiper@2018 com total acesso no servidor.
 * Caso não seja possível criar o usuário solicitado, certificar-se de que o arquivo appsettings.json (projeto PortariaRemotaAPI) está devidamente configurado com o endereço, porta, usuário e senha do banco MySql disponível para uso.

  Via prompt de comandos:
    1 - Acessar diretório PortariaRemotaAPI.
      * - Executar comando: > dotnet ef database update . Este comando é responsável por criar a base dados e configurá-la com os dados iniciais.
      * - Executar comando: > dotnet run . Este comando inicia a API, liberando seu acesso para Localhost:5000 (http) e Locahost:5001 (https). Para testar a API e seu funcionamento você pode abrir um browser e acessar o endereço: https://localhost:5001/swagger -- O swagger foi usado para geração automática da documentação e uso da API.
    2 - Abrir um segundo prompt de comandos e acessar o diretório WebPortariaRemota.
      * - Executar o comando: > dotnet run. Este comando inicia a API, liberando seu acesso para Localhost:5002 (http) e Locahost:5003 (https). Para acessar o site basta usar o endereço: https://localhost:5003


   O web site foi criado com as seguintes páginas:

   Home -> Usada apenas como página de apresentação.
   Apartamentos -> Usada para gerenciamento dos apartamentos, nesta página está disponível o acesso à visualização, inserção, alteração e exclusão de registros.
   Moradores -> Usada para gerenciamento dos moradores, nesta página está disponível o acesso à visualização, inserção, alteração e exclusão de registros.
   * Todas as páginas, exceto Home, exigem que o usuário esteja logado no sistema, para isso foi criado o login:
     User : Kiper
     Senha: Kiper@2018
   * Não foi disponibilizada uma página para cadastro de novos usuários, levando em consideração que isto tornaria mais complexa a implementação, visto que usuários possuem diferentes perfis e não foi definido como seria estes perfis em relação ao acesso dos cadastros, listas, etc.


    Os Testes:
    Foi criado um projeto de Testes usando o XUnit. Para executar o projeto use os passos abaixo:
    1 - Abra um novo prompt de comando e acesse o diretório TestPortariaRemotaApi\bin\Debug\netcoreapp2.1.
      * - Executar o comando: > dotnet vstest TestPortariaRemotaApi.dll -lt
          Este comando é responsável por listar todos os testes disponíveis no projeto. Foi criado um total de 20 testes para o projeto da API.
      * - Executar o comando: > dotnet vstest TestPortariaRemotaApi.dll
          Este comando executa todos os testes e ao final é listado apenas o resultado de quantos testes deram OK e quantos falharam.
