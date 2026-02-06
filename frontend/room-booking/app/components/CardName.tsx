interface Props {
    name: string;
    capacity: number;
}

export const CardName = ( {name, capacity}: Props ) => {
    return(
        <div style={{
            display: "flex",
            flexDirection: "row",
            alignItems: "center",
            justifyContent: "center",
            // padding: "1rem",
            // border: "1px solid #ccc",
            // borderRadius: "0.5rem",
        }}>
            <p className="card__name">{name}</p>
            <p className="card__capacity">{capacity} people</p>
        </div>
    )
}