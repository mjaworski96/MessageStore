CREATE TABLE alias
(
	id INTEGER NOT NULL,
	name VARCHAR(100) NOT NULL,
	internal BOOLEAN NOT NULL
);
ALTER TABLE alias ADD CONSTRAINT alias_pkey PRIMARY KEY (id);
CREATE SEQUENCE public.alias_id_seq
    AS INTEGER
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
ALTER SEQUENCE alias_id_seq OWNED BY alias.id;
ALTER TABLE alias ALTER COLUMN id SET DEFAULT nextval('public.alias_id_seq'::regclass);
ALTER TABLE alias ADD CONSTRAINT alias_con_unq_name UNIQUE (name);

CREATE TABLE writer_type
(
	id INTEGER NOT NULL,
	name VARCHAR(10) NOT NULL
);
ALTER TABLE writer_type ADD CONSTRAINT writer_type_pkey PRIMARY KEY (id);
CREATE SEQUENCE public.writer_type_id_seq
    AS INTEGER
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
ALTER SEQUENCE writer_type_id_seq OWNED BY writer_type.id;
ALTER TABLE writer_type ALTER COLUMN id SET DEFAULT nextval('public.writer_type_id_seq'::regclass);
ALTER TABLE writer_type ADD CONSTRAINT writer_type_con_unq_name UNIQUE (name);

CREATE TABLE app_user
(
	id INTEGER NOT NULL,
	username VARCHAR(20) NOT NULL,
	password VARCHAR(60) NOT NULL
);
ALTER TABLE app_user ADD CONSTRAINT app_user_pkey PRIMARY KEY (id);
CREATE SEQUENCE public.app_user_id_seq
    AS INTEGER
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
ALTER SEQUENCE app_user_id_seq OWNED BY app_user.id;
ALTER TABLE app_user ALTER COLUMN id SET DEFAULT nextval('public.app_user_id_seq'::regclass);
ALTER TABLE app_user ADD CONSTRAINT app_user_con_unq_name UNIQUE (username);

CREATE TABLE application
(
	id INTEGER NOT NULL,
	name VARCHAR(20) NOT NULL
);
ALTER TABLE application ADD CONSTRAINT application_pkey PRIMARY KEY (id);
CREATE SEQUENCE public.application_id_seq
    AS INTEGER
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
ALTER SEQUENCE application_id_seq OWNED BY application.id;
ALTER TABLE application ALTER COLUMN id SET DEFAULT nextval('public.application_id_seq'::regclass);
ALTER TABLE application ADD CONSTRAINT application_con_unq_name UNIQUE (name);

CREATE TABLE contact
(
	id INTEGER NOT NULL,
	name VARCHAR(20) NOT NULL,
	app_user_id INTEGER NOT NULL,
	application_id INTEGER NOT NULL,
	in_application_id VARCHAR(100)
);
ALTER TABLE contact ADD CONSTRAINT contact_pkey PRIMARY KEY (id);
CREATE SEQUENCE public.contact_id_seq
    AS INTEGER
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
ALTER SEQUENCE contact_id_seq OWNED BY contact.id;
ALTER TABLE contact ALTER COLUMN id SET DEFAULT nextval('public.contact_id_seq'::regclass);

ALTER TABLE contact ADD CONSTRAINT fk_contact_app_user FOREIGN KEY (app_user_id) REFERENCES app_user(id) ON DELETE CASCADE;
ALTER TABLE contact ADD CONSTRAINT fk_contact_application FOREIGN KEY (application_id) REFERENCES application(id) ON DELETE CASCADE;

CREATE TABLE alias_member
(
	id INTEGER NOT NULL,
	alias_id INTEGER NOT NULL,
	contact_id INTEGER NOT NULL
);
ALTER TABLE alias_member ADD CONSTRAINT alias_member_pkey PRIMARY KEY (id);
CREATE SEQUENCE public.alias_member_id_seq
    AS INTEGER
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
ALTER SEQUENCE alias_member_id_seq OWNED BY alias_member.id;
ALTER TABLE alias_member ALTER COLUMN id SET DEFAULT nextval('public.alias_member_id_seq'::regclass);

ALTER TABLE alias_member ADD CONSTRAINT fk_alias_member_alias FOREIGN KEY (alias_id) REFERENCES alias(id) ON DELETE CASCADE;
ALTER TABLE alias_member ADD CONSTRAINT fk_alias_member_contact FOREIGN KEY (contact_id) REFERENCES contact(id) ON DELETE CASCADE;

CREATE TABLE message
(
	id INTEGER NOT NULL,
	content VARCHAR(1000),
	attachment BYTEA,
	date TIMESTAMP,
	writer_type_id INTEGER NOT NULL,
	contact_id INTEGER NOT NULL
);
ALTER TABLE message ADD CONSTRAINT message_pkey PRIMARY KEY (id);
CREATE SEQUENCE public.message_id_seq
    AS INTEGER
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
ALTER SEQUENCE message_id_seq OWNED BY message.id;
ALTER TABLE message ALTER COLUMN id SET DEFAULT nextval('public.message_id_seq'::regclass);

ALTER TABLE message ADD CONSTRAINT fk_message_writer_type FOREIGN KEY (writer_type_id) REFERENCES writer_type(id) ON DELETE CASCADE;
ALTER TABLE message ADD CONSTRAINT fk_message_contact FOREIGN KEY (contact_id) REFERENCES contact(id) ON DELETE CASCADE;

INSERT INTO app_user(username, password) VALUES
('test', '$2a$10$hbnIe4MWaMmiL6eWHRqYFu2n.HBs9DtfG33tm.Qct13t9vqzYCfEO'), --test
('marcin', '$2a$10$XxY/vqp0dT3sQPRvcdDDM.lnpT6Q3SJF3lWN3HLmeT4rdmZ8nHtQe'); --test

INSERT INTO writer_type(name) VALUES
('app_user'), ('writer');

INSERT INTO application(name) VALUES
('sms'), ('messenger');
