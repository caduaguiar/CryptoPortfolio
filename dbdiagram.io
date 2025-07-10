// use dbml to define your database structure
// docs: https://dbml.dbdiagram.io/docs

Table users {
  id integer [pk] // Identificador único do usuário.
  username varchar [not null] // Nome de usuário.
  email varchar [not null, unique] // Endereço de e-mail do usuário, único.
  password_hash varchar [not null] // Hash da senha para segurança.
  created_at timestamp [default: `now()`] // Data e hora de criação do registro.
  last_updated timestamp [default: `now()`] // Data e hora da última atualização do registro.
}

Table portfolios {
  id integer [pk] // Identificador único do portfólio.
  user_id integer [not null] // Chave estrangeira para o usuário proprietário do portfólio.
  name varchar [not null] // Nome do portfólio (ex: "Portfólio Principal", "Investimentos de Longo Prazo", "Bens Pessoais").
  description text // Descrição detalhada do portfólio.
  base_currency varchar [not null] // Moeda base para o portfólio (ex: "USD", "BRL").
  created_at timestamp [default: `now()`] // Data e hora de criação do registro.
  last_updated timestamp [default: `now()`] // Data e hora da última atualização do registro.
  is_active boolean [default: true] // Indica se o portfólio está ativo.
}

Table asset_types {
  id integer [pk] // Identificador único do tipo de ativo.
  name varchar [not null, unique] // Nome do tipo de ativo (ex: 'Cryptocurrency', 'Stock', 'Real Estate', 'Vehicle', 'Commodity', 'Collectibles').
  description text // Descrição do tipo de ativo.
}

Table assets {
  id integer [pk] // Identificador único do ativo.
  portfolio_id integer [not null] // Chave estrangeira para o portfólio ao qual o ativo pertence.
  asset_type_id integer [not null] // Chave estrangeira para o tipo de ativo (ex: Crypto, Stock, Car).
  name varchar [not null] // Nome do ativo (ex: "Bitcoin", "Ações da Tesla", "Minha Casa", "Ford Mustang 1969").
  symbol varchar // Opcional: Símbolo do ativo (ex: "BTC", "TSLA"). Mais relevante para criptos/ações.
  description text // Descrição detalhada do ativo (ex: modelo do carro, endereço do terreno, detalhes da cripto).
  quantity decimal [not null] // Quantidade do ativo. Para itens únicos (carro, terreno), use 1. Para criptos/ações, use a quantidade de unidades.
  acquisition_date date [not null] // Data de aquisição do ativo.
  acquisition_cost decimal [not null] // Custo total de aquisição do ativo.
  current_value decimal // Valor estimado atual total do ativo. Para ativos fungíveis (cripto/ações), seria (preço por unidade * quantidade). Para ativos únicos (carro/terreno), seria o valor total estimado. Este campo deve ser atualizado periodicamente.
  currency varchar [not null] // Moeda em que o valor do ativo é denominado (ex: USD, BRL).
  location varchar // Opcional: Localização física do ativo (ex: endereço do terreno, local de guarda do carro).
  notes text // Quaisquer notas adicionais sobre o ativo.
  last_updated timestamp [default: `now()`] // Data e hora da última atualização do registro.
  created_at timestamp [default: `now()`] // Data e hora de criação do registro.
  is_active boolean [default: true] // Indica se o ativo está ativo no portfólio.
}

Table transactions {
  id integer [pk] // Identificador único da transação.
  asset_id integer [not null] // Chave estrangeira para o ativo envolvido na transação.
  portfolio_id integer [not null] // Chave estrangeira para o portfólio ao qual a transação pertence (para facilitar consultas).
  transaction_type varchar [not null] // Tipo da transação (ex: 'BUY', 'SELL', 'DEPOSIT', 'WITHDRAWAL', 'FEE', 'DIVIDEND', 'MAINTENANCE', 'IMPROVEMENT', 'VALUATION').
  amount decimal [not null] // Quantidade de unidades envolvidas na transação (ex: 0.1 BTC comprado, 5 ações vendidas).
  price_per_unit decimal // Preço por unidade no momento da transação.
  total_transaction_value decimal [not null] // Valor total da transação (ex: (quantidade * preço por unidade) ou custo total de uma manutenção).
  transaction_currency varchar [not null] // Moeda em que a transação foi realizada (ex: USD, BRL).
  transaction_date timestamp [not null] // Data e hora da transação.
  notes text // Quaisquer notas adicionais sobre a transação.
  created_at timestamp [default: `now()`] // Data e hora de criação do registro.
}

// Relações
Ref: users.id < portfolios.user_id
Ref: portfolios.id < assets.portfolio_id
Ref: asset_types.id < assets.asset_type_id
Ref: assets.id < transactions.asset_id
Ref: portfolios.id < transactions.portfolio_id
