CREATE TABLE CAMINE( 
    id_camin int primary key, 
    adresa varchar(50), 
    nr_camere int not null, 
    nr_locuri int  not null,
    facultate varchar(30) not null,
    descriere varchar(100)
); 

CREATE TABLE CAMERE( 
    id_camera int primary key, 
    id_camin int foreign key references CAMINE(id_camin), 
    limita_nr_studenti int not null, 
    nr_studenti_cazati int not null, 
    descriere varchar(50), 
    nr_camera int  not null
); 

CREATE TABLE STUDENT( 
    id_student int primary key, 
    nume varchar(50) not null, 
    prenume varchar(50) not null, 
    facultate varchar(50) not null, 
    varsta int not null, 
    adresa varchar(50) not null, 
    email varchar(50) not null, 
    an int, 
    id_camera int foreign key references CAMERE(id_camera), 
    data_cazare date
); 

CREATE TABLE ADMINISTRATORI ( 
    id_admin int primary key, 
    id_camin int foreign key references CAMINE(id_camin), 
    nume varchar(100) not null, 
    adresa varchar(50) not null, 
    nr_telefon varchar(10) not null, 
    email varchar(100) not null
); 

CREATE TABLE TICHET( 
    id_tichet int primary key, 
    id_student int foreign key references STUDENT(id_student), 
    data_emitere date NOT NULL, 
    date_rezolvare date, 
    status_tichet bit NOT NULL, 
    detalii varchar(200) NOT NULL, 
    tip_tichet bit NOT NULL ,
    id_camera int not null foreign key references CAMERE(id_camera),
    feedback varchar(100),
    file_name varchar(100)
); 

CREATE TABLE APPLICANT(
	id_applicant int PRIMARY KEY,
	nume varchar(30) NOT NULL,
	prenume varchar(30) NOT NULL,
	facultate varchar(40) NOT NULL,
	varsta int NOT NULL,
	adresa varchar(50) NOT NULL,
	email varchar(50) NOT NULL,
	an INT NOT NULL
);