FROM node:slim as builder

COPY . /src

WORKDIR /src

RUN npm install
RUN npm run build

FROM nginx

COPY --from=builder /src/dist/disp /usr/share/nginx/html
COPY default.conf /etc/nginx/conf.d