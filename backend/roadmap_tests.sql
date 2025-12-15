---- SHEMA
--CREATE SCHEMA IF NOT EXISTS roadmap_test;

--CREATE TABLE roadmap_test.topics
--(
--    id bigserial PRIMARY KEY,
--    external_id text NOT NULL,
--    name text NOT NULL,
--    description text,
--    created_at timestamptz DEFAULT now()
--);

--CREATE TABLE roadmap_test.questions
--(
--    id bigserial PRIMARY KEY,
--    external_id text NOT NULL,
--    text text NOT NULL,
--    difficulty text NOT NULL,
--    type text NOT NULL,
--    answers jsonb NOT NULL,
--    created_at timestamptz DEFAULT now()
--);

--CREATE EXTENSION IF NOT EXISTS pg_trgm; -- NOT clear??

--CREATE UNIQUE INDEX IF NOT EXISTS uq_topics_external_id
--ON roadmap_test.topics (external_id);

--CREATE UNIQUE INDEX IF NOT EXISTS uq_questions_external_id
--ON roadmap_test.questions (external_id);

--CREATE INDEX IF NOT EXISTS idx_topics_name_trgm
--ON roadmap_test.topics USING gin (name gin_trgm_ops);

--CREATE INDEX IF NOT EXISTS idx_questions_text_trgm
--ON roadmap_test.questions USING gin (text gin_trgm_ops);
