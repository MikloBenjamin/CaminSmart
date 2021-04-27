CREATE TABLE CAMINE( 
    id_camin int primary key, 
    adresa varchar(30), 
    nr_camere int, 
    nr_locuri_normale int, 
    nr_locuri_erasmus int, 
    nr_locuri_social int, 
    facultate varchar(30) 
); 

CREATE TABLE CAMERE( 
    id_camera int primary key, 
    id_camin int foreign key references CAMINE(id_camin), 
    limita_nr_studenti int, 
    nr_studenti_cazati int, 
    descriere varchar(50), 
    tip_camera varchar(20) 
); 

CREATE TABLE STUDENT( 
    id_student int primary key, 
    nume varchar(50), 
    prenume varchar(50), 
    facultate varchar(50), 
    varsta int, 
    adresa varchar(50), 
    email varchar(50), 
    an int, 
    status_cazare bit, 
    id_camera int foreign key references CAMERE(id_camera), 
    data_cazare date, 
    data_decazare date 
); 

CREATE TABLE ADMINISTRATORI ( 
    id_admin int primary key, 
    id_camin int foreign key references CAMINE(id_camin), 
    nume varchar(50) not null, 
    adresa varchar(50) not null, 
    nr_telefon varchar(10) not null, 
    email varchar(20) not null, 
    tip_admin bit not null 
); 

CREATE TABLE TICHET( 
    id_tichet int primary key, 
    id_student int foreign key references STUDENT(id_student), 
    data_emitere date NOT NULL, 
    date_rezolvare date, 
    status_tichet bit NOT NULL, 
    detalii varchar(200) NOT NULL, 
    tip_tichet bit NOT NULL 
); 


CREATE TABLE APPLICANT(
	id_applicant int PRIMARY KEY,
	nume varchar(30) NOT NULL,
	prenume varchar(30) NOT NULL,
	facultate varchar(40) NOT NULL,
	varsta int NOT NULL,
	adresa varchar(50) NOT NULL,
	email varchar(50) NOT NULL,
	an INT NOT NULL,
    file_path varchar(100) NOT NULL
);