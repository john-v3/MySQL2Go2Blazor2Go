package main

import (
	"database/sql"
	"encoding/json"
	"fmt"
	"log"
	"net/http"
	"time"

	_ "github.com/go-sql-driver/mysql"
)

var currentDB *sql.DB

type Info_table struct {
	TABLE_CATALOG   string    `json:"TABLE_CATALOG "`
	TABLE_SCHEMA    string    `json:"TABLE_SCHEMA"`
	TABLE_NAME      string    `json:"TABLE_NAME"`
	TABLE_TYPE      string    `json:"TABLE_TYPE"`
	ENGINE          string    `json:"ENGINE"`
	VERSION         *int64    `json:"VERSION"`
	ROW_FORMAT      string    `json:"ROW_FORMAT"`
	TABLE_ROWS      *int64    `json:"TABLE_ROWS"`
	AVG_ROW_LENGTH  *int64    `json:"AVG_ROW_LENGTH"`
	DATA_LENGTH     *int64    `json:"DATA_LENGTH"`
	MAX_DATA_LENGTH *int64    `json:"MAX_DATA_LENGTH"`
	INDEX_LENGTH    *int64    `json:"INDEX_LENGTH"`
	DATA_FREE       *int64    `json:"DATA_FREE"`
	AUTO_INCREMENT  *int64    `json:"AUTO_INCREMENT"`
	CREATE_TIME     time.Time `json:"CREATE_TIME"`
	UPDATE_TIME     time.Time `json:"UPDATE_TIME"`
	CHECK_TIME      time.Time `json:"CHECK_TIME"`
	TABLE_COLLATION string    `json:"TABLE_COLLATION"`
	CHECKSUM        *int64    `json:"CHECKSUM"`
	CREATE_OPTIONS  string    `json:"CREATE_OPTIONS"`
	TABLE_COMMENT   string    `json:"TABLE_COMMENT"`
}

type Info_column struct {
	TABLE_CATALOG            string `json:"TABLE_CATALOG"`
	TABLE_SCHEMA             string `json:"TABLE_SCHEMA"`
	TABLE_NAME               string `json:"TABLE_NAME"`
	COLUMN_NAME              string `json:"COLUMN_NAME"`
	ORDINAL_POSITION         int    `json:"ORDINAL_POSITION"`
	COLUMN_DEFAULT           string `json:"COLUMN_DEFAULT"`
	IS_NULLABLE              string `json:"IS_NULLABLE"`
	DATA_TYPE                string `json:"DATA_TYPE"`
	CHARACTER_MAXIMUM_LENGTH *int64 `json:"CHARACTER_MAXIMUM_LENGTH"`
	CHARACTER_OCTET_LENGTH   *int64 `json:"CHARACTER_OCTET_LENGTH"`
	NUMERIC_PRECISION        int    `json:"NUMERIC_PRECISION"`
	NUMERIC_SCALE            int    `json:"NUMERIC_SCALE"`
	DATETIME_PRECISION       int    `json:"DATETIME_PRECISION"`
	CHARACTER_SET_NAME       string `json:"CHARACTER_SET_NAME"`
	COLLATION_NAME           string `json:"COLLATION_NAME"`
	COLUMN_TYPE              string `json:"COLUMN_TYPE"`
	COLUMN_KEY               string `json:"COLUMN_KEY"`
	EXTRA                    string `json:"EXTRA"`
	PRIVILEGES               string `json:"PRIVILEGES"`
	COLUMN_COMMENT           string `json:"COLUMN_COMMENT"`
	GENERATION_EXPRESSION    string `json:"GENERATION_EXPRESSION"`
	SRS_ID                   int    `json:"SRS_ID"`
}

func dsn() string {
	const (
		username = "root"
		password = ""
		hostname = ""
	)
	return fmt.Sprintf("%s:%s@tcp(%s)/?parseTime=true", username, password, hostname)
}

func GetTables(w http.ResponseWriter, r *http.Request) {
	var table Info_table
	var tablearray []Info_table

	output, err := currentDB.Query("SELECT * FROM information_schema.TABLES WHERE TABLE_SCHEMA NOT IN ('performance_schema', 'mysql', 'sys', 'information_schema')")

	if err != nil {
		panic(err)
	}

	for output.Next() {
		output.Scan(&table.TABLE_CATALOG, &table.TABLE_SCHEMA, &table.TABLE_NAME, &table.TABLE_TYPE, &table.ENGINE, &table.VERSION, &table.ROW_FORMAT, &table.TABLE_ROWS, &table.AVG_ROW_LENGTH, &table.DATA_LENGTH, &table.MAX_DATA_LENGTH, &table.INDEX_LENGTH, &table.DATA_FREE, &table.AUTO_INCREMENT, &table.CREATE_TIME, &table.UPDATE_TIME, &table.CHECK_TIME, &table.TABLE_COLLATION, &table.CHECKSUM, &table.CREATE_OPTIONS, &table.TABLE_COMMENT)

		fmt.Printf("%v\n", table)

		tablearray = append(tablearray, table)
	}
	trueOutput, err := json.Marshal(tablearray)
	if err != nil {
		panic(err)
	}
	defer r.Body.Close()

	w.Header().Set("Access-Control-Allow-Origin", "*")
	w.Header().Set("Access-Control-Allow-Headers", "Content-Type")
	w.Write(trueOutput)
}

func GetColumnsFromTable(w http.ResponseWriter, r *http.Request) {
	var column Info_column
	var columnarray []Info_column

	output, err := currentDB.Query("select COALESCE(TABLE_CATALOG,'') AS TABLE_CATALOG,COALESCE(TABLE_SCHEMA,'') AS TABLE_SCHEMA,COALESCE(TABLE_NAME,'') AS TABLE_NAME,COALESCE(COLUMN_NAME,'') AS COLUMN_NAME,COALESCE(ORDINAL_POSITION,0) AS ORDINAL_POSITION,COALESCE(COLUMN_DEFAULT,'') AS COLUMN_DEFAULT,COALESCE(IS_NULLABLE,'') AS IS_NULLABLE,COALESCE(DATA_TYPE,'') AS DATA_TYPE,COALESCE(CHARACTER_MAXIMUM_LENGTH,0) AS CHARACTER_MAXIMUM_LENGTH,COALESCE(CHARACTER_OCTET_LENGTH,0) AS CHARACTER_OCTET_LENGTH,COALESCE(NUMERIC_PRECISION,0) AS NUMERIC_PRECISION,COALESCE(NUMERIC_SCALE,0) AS NUMERIC_SCALE,COALESCE(DATETIME_PRECISION,0) AS DATETIME_PRECISION,COALESCE(CHARACTER_SET_NAME,'') AS CHARACTER_SET_NAME,COALESCE(COLLATION_NAME,'') AS COLLATION_NAME,COALESCE(COLUMN_TYPE,'') AS COLUMN_TYPE,COALESCE(COLUMN_KEY,'') AS COLUMN_KEY,COALESCE(EXTRA,'') AS EXTRA,COALESCE(PRIVILEGES,'') AS PRIVILEGES,COALESCE(COLUMN_COMMENT,'') AS COLUMN_COMMENT,COALESCE(GENERATION_EXPRESSION,'') AS GENERATION_EXPRESSION,COALESCE(SRS_ID,0) AS SRS_ID from information_schema.`COLUMNS`  where TABLE_SCHEMA = ? AND TABLE_NAME = ?;", r.URL.Query()["schema"][0], r.URL.Query()["table"][0])
	if err != nil {
		panic(err)
	}

	for output.Next() {
		output.Scan(&column.TABLE_CATALOG, &column.TABLE_SCHEMA, &column.TABLE_NAME, &column.COLUMN_NAME, &column.ORDINAL_POSITION, &column.COLUMN_DEFAULT, &column.IS_NULLABLE, &column.DATA_TYPE, &column.CHARACTER_MAXIMUM_LENGTH, &column.CHARACTER_OCTET_LENGTH, &column.NUMERIC_PRECISION, &column.NUMERIC_SCALE, &column.DATETIME_PRECISION, &column.CHARACTER_SET_NAME, &column.COLLATION_NAME, &column.COLUMN_TYPE, &column.COLUMN_KEY, &column.EXTRA, &column.PRIVILEGES, &column.COLUMN_COMMENT, &column.GENERATION_EXPRESSION, &column.SRS_ID)
		fmt.Printf("%v\n", column)

		columnarray = append(columnarray, column)
	}

	trueOutput, err := json.Marshal(columnarray)
	if err != nil {
		panic(err)
	}

	defer r.Body.Close()
	w.Header().Set("Access-Control-Allow-Origin", "*")
	w.Header().Set("Access-Control-Allow-Headers", "Content-Type")
	w.Write(trueOutput)
}

func main() {
	// we need to implement a rest api
	db, err := sql.Open("mysql", dsn())
	if err != nil {
		panic(err)
	}
	// See "Important settings" section.
	db.SetConnMaxLifetime(time.Minute * 3)
	db.SetMaxOpenConns(10)
	db.SetMaxIdleConns(10)

	currentDB = db

	http.HandleFunc("/tables", GetTables)
	http.HandleFunc("/columns", GetColumnsFromTable)
	print("listening...")
	log.Fatal(http.ListenAndServe(":10000", nil))
}
