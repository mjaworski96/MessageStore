CREATE TABLE aliases
(
	id INTEGER NOT NULL,
	name VARCHAR(100) NOT NULL,
	internal BOOLEAN NOT NULL
);
ALTER TABLE aliases ADD CONSTRAINT aliases_pkey PRIMARY KEY (id);
CREATE SEQUENCE public.aliases_id_seq
    AS INTEGER
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
ALTER SEQUENCE aliases_id_seq OWNED BY aliases.id;
ALTER TABLE aliases ALTER COLUMN id SET DEFAULT nextval('public.aliases_id_seq'::regclass);

CREATE TABLE writer_types
(
	id INTEGER NOT NULL,
	name VARCHAR(10) NOT NULL
);
ALTER TABLE writer_types ADD CONSTRAINT writer_types_pkey PRIMARY KEY (id);
CREATE SEQUENCE public.writer_types_id_seq
    AS INTEGER
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
ALTER SEQUENCE writer_types_id_seq OWNED BY writer_types.id;
ALTER TABLE writer_types ALTER COLUMN id SET DEFAULT nextval('public.writer_types_id_seq'::regclass);
ALTER TABLE writer_types ADD CONSTRAINT writer_types_con_unq_name UNIQUE (name);

CREATE TABLE app_users
(
	id INTEGER NOT NULL,
	username VARCHAR(20) NOT NULL,
	password VARCHAR(60) NOT NULL
);
ALTER TABLE app_users ADD CONSTRAINT app_users_pkey PRIMARY KEY (id);
CREATE SEQUENCE public.app_users_id_seq
    AS INTEGER
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
ALTER SEQUENCE app_users_id_seq OWNED BY app_users.id;
ALTER TABLE app_users ALTER COLUMN id SET DEFAULT nextval('public.app_users_id_seq'::regclass);
ALTER TABLE app_users ADD CONSTRAINT app_users_con_unq_name UNIQUE (username);

CREATE TABLE applications
(
	id INTEGER NOT NULL,
	name VARCHAR(20) NOT NULL
);
ALTER TABLE applications ADD CONSTRAINT applications_pkey PRIMARY KEY (id);
CREATE SEQUENCE public.applications_id_seq
    AS INTEGER
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
ALTER SEQUENCE applications_id_seq OWNED BY applications.id;
ALTER TABLE applications ALTER COLUMN id SET DEFAULT nextval('public.applications_id_seq'::regclass);
ALTER TABLE applications ADD CONSTRAINT applications_con_unq_name UNIQUE (name);

CREATE TABLE contacts
(
	id INTEGER NOT NULL,
	name VARCHAR(100) NOT NULL,
	in_application_id VARCHAR(100),
	app_user_id INTEGER NOT NULL,
	application_id INTEGER NOT NULL
);
ALTER TABLE contacts ADD CONSTRAINT contacts_pkey PRIMARY KEY (id);
CREATE SEQUENCE public.contacts_id_seq
    AS INTEGER
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
ALTER SEQUENCE contacts_id_seq OWNED BY contacts.id;
ALTER TABLE contacts ALTER COLUMN id SET DEFAULT nextval('public.contacts_id_seq'::regclass);

ALTER TABLE contacts ADD CONSTRAINT fk_contacts_app_users FOREIGN KEY (app_user_id) REFERENCES app_users(id) ON DELETE CASCADE;
ALTER TABLE contacts ADD CONSTRAINT fk_contacts_applications FOREIGN KEY (application_id) REFERENCES applications(id) ON DELETE CASCADE;

CREATE TABLE aliases_members
(
	id INTEGER NOT NULL,
	alias_id INTEGER NOT NULL,
	contact_id INTEGER NOT NULL
);
ALTER TABLE aliases_members ADD CONSTRAINT aliases_members_pkey PRIMARY KEY (id);
CREATE SEQUENCE public.aliases_members_id_seq
    AS INTEGER
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
ALTER SEQUENCE aliases_members_id_seq OWNED BY aliases_members.id;
ALTER TABLE aliases_members ALTER COLUMN id SET DEFAULT nextval('public.aliases_members_id_seq'::regclass);

ALTER TABLE aliases_members ADD CONSTRAINT fk_aliases_members_aliases FOREIGN KEY (alias_id) REFERENCES aliases(id) ON DELETE CASCADE;
ALTER TABLE aliases_members ADD CONSTRAINT fk_aliases_members_contacts FOREIGN KEY (contact_id) REFERENCES contacts(id) ON DELETE CASCADE;

CREATE TABLE messages
(
	id INTEGER NOT NULL,
	content VARCHAR(1000),
	date TIMESTAMP,
	writer_type_id INTEGER NOT NULL,
	contact_id INTEGER NOT NULL
);
ALTER TABLE messages ADD CONSTRAINT messages_pkey PRIMARY KEY (id);
CREATE SEQUENCE public.messages_id_seq
    AS INTEGER
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
ALTER SEQUENCE messages_id_seq OWNED BY messages.id;
ALTER TABLE messages ALTER COLUMN id SET DEFAULT nextval('public.messages_id_seq'::regclass);

ALTER TABLE messages ADD CONSTRAINT fk_messages_writer_types FOREIGN KEY (writer_type_id) REFERENCES writer_types(id) ON DELETE CASCADE;
ALTER TABLE messages ADD CONSTRAINT fk_messages_contacts FOREIGN KEY (contact_id) REFERENCES contacts(id) ON DELETE CASCADE;

CREATE TABLE attachments
(
	id INTEGER NOT NULL,
	content BYTEA,
	content_type VARCHAR(100),
	message_id INTEGER NOT NULL
);
ALTER TABLE attachments ADD CONSTRAINT attachments_pkey PRIMARY KEY (id);
CREATE SEQUENCE public.attachments_id_seq
    AS INTEGER
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
ALTER SEQUENCE attachments_id_seq OWNED BY attachments.id;
ALTER TABLE attachments ALTER COLUMN id SET DEFAULT nextval('public.attachments_id_seq'::regclass);

ALTER TABLE attachments ADD CONSTRAINT fk_attachments_messages FOREIGN KEY (message_id) REFERENCES messages(id) ON DELETE CASCADE;

INSERT INTO app_users(username, password) VALUES
('test', '$2a$10$hbnIe4MWaMmiL6eWHRqYFu2n.HBs9DtfG33tm.Qct13t9vqzYCfEO'), --test
('marcin', '$2a$10$XxY/vqp0dT3sQPRvcdDDM.lnpT6Q3SJF3lWN3HLmeT4rdmZ8nHtQe'); --test

INSERT INTO writer_types(name) VALUES
('app_user'), ('contact');

INSERT INTO applications(name) VALUES
('sms'), ('messenger');
