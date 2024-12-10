export default function Navbar(): JSX.Element {

    // Styles
    const containerStyle = "flex justify-between items-center px-4 py-2 bg-gray-800 text-white";
    const titleStyle = "text-xl font-bold";
    const navStyle = "flex space-x-4";
    const listItemStyle = "list-none";

    // Menu
    const menu = ["Menu 1", "Menu 2", "Menu 3"];

    return (
        <div className={containerStyle}>
            <h1 className={titleStyle}>Mon Titre</h1>
            <nav>
                <ul className={navStyle}>
                    {menu.map((item, index) => (
                        <li key={index} className={listItemStyle}>
                            {item}
                        </li>
                    ))}
                </ul>
            </nav>
        </div>
    );
}

