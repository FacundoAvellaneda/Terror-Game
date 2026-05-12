Acá estan todos los scripts que se necesitan para el sistema de cinematicas.

basicamente tenemos algo llamado pasos o Steps, estos steps son las secuencias que sigue el codigo para llegar hasta un lugar.

Tambien tenemos cinematicData que es basicamente el conjunto de pasos que un objeto puede dar 
Con los steps tenemos uno llamado paralel que, como dice el nombre, es para ejecutar dos tipos de steps al mismo tiempo,
por ejemplo si yo quiero que algo se mueva utilizo el move Steps con el id del objeto que quiero mover y si quiero que la camara
siga a ese objeto entonces tengo que añadir a mi cinematic data un Parallel step que me permite agregar en secuencia cualquier step con otro, solo faltaria
agregarle el id del objeto que quiero seguir y cambiar el modo a follow target.
luego tenemos el cinematic actor, que es para definir un id a algun objeto que querramos usar en alguna cinematica(hay que escribir el mismo id si queremos que la camara lo siga)
