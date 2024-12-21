3-layer-arch / console

to init db
docker exec -t postgres_test pg_restore -U test -d test_db /backup/main_postgres.dump

main_postgres.dump locate in project
