CREATE TABLE statuses
(
	id INTEGER NOT NULL,
	name VARCHAR(256) NOT NULL
);
ALTER TABLE statuses ADD CONSTRAINT statuses_pkey PRIMARY KEY (id);
CREATE SEQUENCE public.statuses_id_seq
    AS INTEGER
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
ALTER SEQUENCE statuses_id_seq OWNED BY statuses.id;
ALTER TABLE statuses ALTER COLUMN id SET DEFAULT nextval('public.statuses_id_seq'::regclass);

CREATE TABLE imports
(
	id VARCHAR(64) NOT NULL,
	status_id INTEGER NOT NULL,
	start_date TIMESTAMP,
	end_date TIMESTAMP,
	user_id INT NOT NULL,
	fb_username VARCHAR(256) NOT NULL
);
ALTER TABLE imports ADD CONSTRAINT imports_pkey PRIMARY KEY (id);
ALTER TABLE imports ADD CONSTRAINT fk_imports_statuses FOREIGN KEY (status_id) REFERENCES statuses(id);
