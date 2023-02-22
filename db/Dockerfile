FROM mcr.microsoft.com/mssql/server:latest

ARG ACCEPT_EULA
ARG SA_PASSWORD
ARG MSSQL_PID

ENV CHECK_POLICY=OFF

COPY ./db/scripts/create-db.sql /tmp/create-db.sql

RUN ( /opt/mssql/bin/sqlservr --accept-eula & ) | grep -q "Service Broker manager has started" \
    && /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P ${SA_PASSWORD} -i /tmp/create-db.sql \
    && pkill sqlservr

CMD ["/opt/mssql/bin/sqlservr"]