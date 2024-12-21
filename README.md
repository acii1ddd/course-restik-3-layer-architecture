3-layer-arch / console

to init db
docker exec -t postgres pg_restore -U test -d restaurant /backup/main_postgres.dump

main_postgres.dump locate in project
